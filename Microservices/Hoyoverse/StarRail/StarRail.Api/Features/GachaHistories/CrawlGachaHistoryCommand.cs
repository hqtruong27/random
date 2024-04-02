﻿//using System.Text.Json;
//using Infrastructure.Core.Dispatchers;
//using Microsoft.Extensions.Logging;
//using MongoDB.Bson;
//using StarRail.Api.Features.Models;
//using StarRail.Domain.Entities;
//using StarRail.Domain.Enums;
//using StarRail.Domain.Interfaces.Repositories;
//using StarRail.Shared.Helpers;

//namespace StarRail.Api.Features.GachaHistories;

//public sealed record CrawlGachaHistoryCommand(string Url) : ICommand<int>
//{
//    public class CrawlGachaHistoryCommandHandler(IRepository<GachaHistory, ObjectId> repository
//        , IMapper mapper
//        , ILogger<CrawlGachaHistoryCommandHandler> logger) : ICommandHandler<CrawlGachaHistoryCommand, int>
//    {
//        public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
//        {
//            logger.LogInformation("Start: crawl {url}", request.Url);

//            var queryString = UrlQueryHelper.Populate<UrlQuery>(request.Url);
//            var total = 0;

//            var gachaHistories = await GetGachaHistoriesAsync(configure.GachaUrl, queryString);
//            if (gachaHistories.Count > 0)
//            {
//                await repository.BulkInsertAsync(gachaHistories);
//            }

//            logger.LogInformation("End: crawl success {total}", gachaHistories.Count);
//            return total;
//        }
//        private async Task<List<GachaHistory>> GetGachaHistoriesAsync(string gachaUrl, UrlQuery qs)
//        {
//            var hasMoreRecords = true;
//            List<GachaHistory> result = [];
//            foreach (var gachaType in Enum.GetValues<GachaType>().ToList())
//            {
//                long endId = 0;
//                long beginId = await GetLastIdAsync(gachaType);
//                do
//                {
//                    var response = await PostAsync(new(), beginId, endId, gachaUrl, qs, gachaType);
//                    var gachaHistories = response.GachaHistories;
//                    switch (gachaHistories.Count)
//                    {
//                        case > 0:
//                            result.AddRange(gachaHistories.Select(mapper.Map<GachaHistory>));
//                            if (beginId == 0)
//                            {
//                                endId = gachaHistories[gachaHistories.Count - 1].Id;
//                            }
//                            else
//                            {
//                                beginId = gachaHistories[0].Id;
//                            }
//                            hasMoreRecords = true;
//                            break;
//                        default:
//                            hasMoreRecords = false;
//                            break;
//                    }

//                    await Task.Delay(500);
//                } while (hasMoreRecords);
//            }

//            return [.. result.OrderBy(x => x.Time)];
//        }
//        private async Task<long> GetLastIdAsync(GachaType gachaType)
//        {
//            var gachaHistories = repository.Queries.Where(x => x.GachaType == gachaType);
//            return !await gachaHistories.AnyAsync() ? 0 : await gachaHistories.MaxAsync(x => x.ReferenceId);
//        }
//        private static async Task<GachaInfoDataResponse> PostAsync(HttpClient client
//           , long beginId
//           , long endId
//           , string gachaUrl
//           , UrlQuery qs
//           , GachaType gachaType)
//        {
//            var requestUri = $"{gachaUrl}?{qs.ToQueryParams(gachaType, beginId, endId)}";

//            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

//            var stream = await response.Content.ReadAsStreamAsync();
//            var gachaInfo = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);

//            if (gachaInfo?.Code == Genshin.Code.AuthKeyTimeOut)
//            {
//                throw new Exception("Authkey timeout");
//            }

//            if (gachaInfo?.Code == Genshin.Code.VisitTooFrequently)
//            {
//                await PostAsync(client, beginId, endId, gachaUrl, qs, gachaType);
//            }

//            return gachaInfo?.Data ?? new();
//        }
//    }
//}
