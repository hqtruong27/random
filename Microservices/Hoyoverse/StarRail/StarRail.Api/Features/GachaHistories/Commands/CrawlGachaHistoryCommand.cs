using Common.Constants;
using Infrastructure.Core.Dispatchers;
using Infrastructure.Core.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Random.ExternalService.Models.Request;
using Random.ExternalService.Models.Response;
using Random.ExternalService.Services;
using StarRail.Api.Mappers;
using StarRail.Core.Entities;
using StarRail.Core.Enums;
using StarRail.Core.Interfaces.Repositories;
using StarRail.Shared.Helpers;

namespace StarRail.Api.Features.GachaHistories.Commands;

public sealed record CrawlGachaHistoryCommand(string Url) : ICommand<int>
{
    public class CrawlGachaHistoryCommandHandler(
        IRepository<GachaHistory, ObjectId> repository,
        ILogger<CrawlGachaHistoryCommandHandler> logger,
        IStarRailService starRailService) : ICommandHandler<CrawlGachaHistoryCommand, int>
    {
        public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start: crawl {url}", request.Url);
            var queryString = UrlQueryHelper.Populate<GetGachaHistoryRequest>(request.Url);

            var gachaHistories = await GetGachaHistoriesAsync(queryString);
            if (gachaHistories.Count > 0)
            {
                await repository.BulkInsertAsync(gachaHistories);
            }

            logger.LogInformation("End: crawl success {total}", gachaHistories.Count);
            return gachaHistories.Count;
        }
        private async Task<List<GachaHistory>> GetGachaHistoriesAsync(GetGachaHistoryRequest request)
        {
            var hasMoreRecords = true;
            List<GachaHistory> result = [];
            foreach (var gachaType in Enum.GetValues<GachaType>().ToList())
            {
                long endId = 0;
                long beginId = await GetLastIdAsync(gachaType);
                do
                {
                    request.Update((int)gachaType, beginId, endId);

                    var gachaHistories = (await GetAsync(request)).Items;
                    switch (gachaHistories.Count)
                    {
                        case > 0:
                            result.AddRange(gachaHistories.Select(ProfileMapper.Mapper.Map<GachaHistory>));
                            if (beginId == 0)
                            {
                                endId = gachaHistories[gachaHistories.Count - 1].Id;
                            }
                            else
                            {
                                beginId = gachaHistories[0].Id;
                            }
                            hasMoreRecords = true;
                            break;
                        default:
                            hasMoreRecords = false;
                            break;
                    }

                    await Task.Delay(500);
                } while (hasMoreRecords);
            }

            return [.. result.OrderBy(x => x.Time)];
        }

        private async Task<long> GetLastIdAsync(GachaType gachaType)
        {
            var gachaHistories = repository.Queries.Where(x => x.GachaType == gachaType);
            return !await gachaHistories.AnyAsync() ? 0 : await gachaHistories.MaxAsync(x => x.ReferenceId);
        }

        private async Task<GachaHistoryDataResponse> GetAsync(GetGachaHistoryRequest request)
        {
            var response = await starRailService.GetGachaHistoriesAsync(request);

            if (response?.Code == Genshin.Code.AuthKeyTimeOut)
            {
                throw new BadRequestException("Authkey timeout");
            }

            if (response?.Code == Genshin.Code.VisitTooFrequently)
            {
                await GetAsync(request);
            }

            return response?.Data ?? new();
        }
    }
}
