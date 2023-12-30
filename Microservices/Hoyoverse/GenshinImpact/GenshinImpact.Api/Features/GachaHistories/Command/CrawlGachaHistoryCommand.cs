using AutoMapper;
using Common.Helpers;
using System.Text.Json;

namespace GenshinImpact.Api.Features.GachaHistories.Command;

public class CrawlGachaHistoryCommand : IRequest<int>
{
    public required string Url { get; set; }

    public class CrawlGachaHistoryCommandHandler(IRepository<GachaHistory, long> repository
        , ISettingRepository _setting
        , IMapper mapper
        , ILogger<CrawlGachaHistoryCommandHandler> logger) : IRequestHandler<CrawlGachaHistoryCommand, int>
    {
        public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start: crawl {url}", request.Url);

            var setting = await _setting.Read<WishListConfig>("WISH_LIST_CONFIG");
            var queryString = UrlQueryHelper.Populate<UrlQuery>(request.Url);
            using var client = new HttpClient();
            long endId = 0;
            var total = 0;
            var batchSize = 5000;
            var hasMoreRecords = true;
            var records = new List<GachaHistory>();
            while (hasMoreRecords)
            {
                var items = await GetRecordsAsync(client, endId, setting.GachaUrl, queryString);
                switch (items.Count)
                {
                    case > 0:
                        records.AddRange(items);
                        endId = items[items.Count - 1].Id;
                        if (records.Count > batchSize)
                        {
                            await repository.BulkInsertAsync(records);
                            records.Clear();
                        }

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
            var requestUri = $"{gachaUrl}?{qs.ToQueryString()}";

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
    }
}
