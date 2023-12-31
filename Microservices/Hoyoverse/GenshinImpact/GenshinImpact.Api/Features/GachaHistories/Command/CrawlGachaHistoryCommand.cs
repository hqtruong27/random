using AutoMapper;
using Common.Enum.Hoyoverse;
using Common.Helpers;
using MongoDB.Driver.Linq;
using System.Text.Json;

namespace GenshinImpact.Api.Features.GachaHistories.Command;

public sealed record CrawlGachaHistoryCommand(string Url) : IRequest<int>
{
    public class CrawlGachaHistoryCommandHandler(IRepository<GachaHistory, long> repository
        , IMapper mapper
        , ISettingRepository setting
        , ILogger<CrawlGachaHistoryCommandHandler> logger) : IRequestHandler<CrawlGachaHistoryCommand, int>
    {
        public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start: crawl {url}", request.Url);

            var queryString = UrlQueryHelper.Populate<UrlQuery>(request.Url);
            var configure = await setting.Read<WishListConfig>("WISH_LIST_CONFIG");
            var total = 0;
            long endId = 0;
            var hasMoreRecords = true;
            using var client = new HttpClient();
            var records = new List<GachaHistory>();
            var lastIdFromDb = await GetLastIdAsync(queryString.GachaType);
            while (hasMoreRecords)
            {
                var items = (await GetRecordsAsync(client, endId, configure.GachaUrl, queryString))
                    .Where(x => x.Id > lastIdFromDb)
                    .ToList();

                switch (items.Count)
                {
                    case > 0:
                        records.AddRange(items);
                        endId = items[items.Count - 1].Id;
                        break;
                    default:
                        hasMoreRecords = false;
                        break;
                }

                total += items.Count;
                await Task.Delay(500, cancellationToken);
            }

            if (records.Count > 0)
            {
                await repository.BulkInsertAsync(records);
            }

            logger.LogInformation("End: crawl success {total}", total);
            return total;
        }

        private async Task<List<GachaHistory>> GetRecordsAsync(HttpClient client, long endId, string gachaUrl, UrlQuery qs)
        {
            var requestUri = $"{gachaUrl}?{qs.ToQueryString(20, endId)}";

            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);
            if (result?.Code == Genshin.Code.AuthKeyTimeOut)
            {
                throw new Exception("Authkey timeout");
            }

            if (result?.Code == Genshin.Code.VisitTooFrequently)
            {
                await GetRecordsAsync(client, endId, gachaUrl, qs);
            }

            return result?.Data.GachaHistories.Select(mapper.Map<GachaHistory>).ToList() ?? [];
        }

        private async Task<long> GetLastIdAsync(int gachaType)
        {
            return await repository
                .Queries.Where(x => x.GachaType == (GachaType)gachaType).MaxAsync(x => x.Id);
        }
    }
}
