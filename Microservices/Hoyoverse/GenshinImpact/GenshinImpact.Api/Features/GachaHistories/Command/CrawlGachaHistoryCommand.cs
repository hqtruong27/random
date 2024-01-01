using AutoMapper;
using Common.Enum.Hoyoverse;
using Common.Helpers;
using DnsClient.Protocol;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Text.Json;
using System.Threading;

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

            var gachaHistories = await GetGachaHistoriesAsync(configure.GachaUrl, queryString);
            if (gachaHistories.Count > 0)
            {
                await repository.BulkInsertAsync(gachaHistories);
            }

            logger.LogInformation("End: crawl success {total}", total);
            return total;
        }

        private async Task<List<GachaHistory>> GetGachaHistoriesAsync(string gachaUrl, UrlQuery qs)
        {
            long endId = 0;
            var hasMoreRecords = true;
            List<GachaHistory> result = [];
            foreach (var gachaType in Enum.GetValues<GachaType>().ToList())
            {
                var lastIdFromDb = await GetLastIdAsync(gachaType);
                do
                {
                    var response = await PostAsync(new(), endId, gachaUrl, qs, gachaType);
                    var gachaHistories = response.GachaHistories.Where(x => x.Id > lastIdFromDb).ToList();
                    switch (gachaHistories.Count)
                    {
                        case > 0:
                            result.AddRange(gachaHistories.Select(mapper.Map<GachaHistory>));
                            endId = gachaHistories[gachaHistories.Count - 1].Id;
                            break;
                        default:
                            hasMoreRecords = false;
                            break;
                    }

                    await Task.Delay(500);
                } while (hasMoreRecords);
            }

            return result;
        }

        private async Task<long> GetLastIdAsync(GachaType gachaType)
        {
            var gachaHistories = repository.Queries.Where(x => x.GachaType == gachaType);
            return !await gachaHistories.AnyAsync() ? 0 : await gachaHistories.MaxAsync(x => x.Id);
        }

        private static async Task<GachaInfoDataResponse> PostAsync(HttpClient client
           , long endId
           , string gachaUrl
           , UrlQuery qs
           , GachaType gachaType)
        {
            var requestUri = $"{gachaUrl}?{qs.ToQueryParams(gachaType, endId)}";

            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            var stream = await response.Content.ReadAsStreamAsync();
            var gachaInfo = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);

            if (gachaInfo?.Code == Genshin.Code.AuthKeyTimeOut)
            {
                throw new Exception("Authkey timeout");
            }

            if (gachaInfo?.Code == Genshin.Code.VisitTooFrequently)
            {
                await PostAsync(client, endId, gachaUrl, qs, gachaType);
            }

            return gachaInfo?.Data ?? new();
        }
    }
}
