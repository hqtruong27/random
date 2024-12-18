namespace Hoyoverse.Features.StarRail.GachaHistories.Commands;

[Tags("StarRail")]
[Post("starrail/crawl-gacha-history")]
public sealed record CrawlGachaHistoryCommand(string Url) : ICommand<int>;

public class CrawlGachaHistoryCommandHandler(
    IRepository<StarRailGachaHistory, ObjectId> repository,
    ILogger<CrawlGachaHistoryCommandHandler> logger,
    IHoyoverseService hoyoverse) : ICommandHandler<CrawlGachaHistoryCommand, int>
{
    public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start: crawl {url}", request.Url);
        var queryString = UrlQueryHelper.Populate<GetGachaHistoryRequest>(request.Url);

        var gachaHistories = await GetGachaHistoriesAsync(queryString);
        if (gachaHistories.Count > 0)
        {
            await repository.BulkInsertAsync(gachaHistories, cancellationToken);
        }

        logger.LogInformation("End: crawl success {total}", gachaHistories.Count);
        return gachaHistories.Count;
    }

    private async Task<List<StarRailGachaHistory>> GetGachaHistoriesAsync(GetGachaHistoryRequest request)
    {
        List<StarRailGachaHistory> result = [];
        foreach (var gachaType in Enum.GetValues<StarRailGachaType>().ToList())
        {
            long endId = 0;
            var hasMoreRecords = true;
            bool scanFirstId = true;
            long beginId = await GetLastIdAsync(gachaType);
            do
            {
                request.Update((int)gachaType, beginId, endId);

                var gachaHistories = (await GetAsync(request, scanFirstId)).Items;
                switch (gachaHistories.Count)
                {
                    case > 0:
                        result.AddRange(gachaHistories.Select(ProfileMapper.Mapper.Map<StarRailGachaHistory>));
                        if (beginId == 0)
                        {
                            endId = gachaHistories[gachaHistories.Count - 1].Id;
                        }
                        else
                        {
                            beginId = gachaHistories[0].Id;
                        }

                        hasMoreRecords = true;
                        scanFirstId = false;
                        break;
                    default:
                        hasMoreRecords = false;
                        break;
                }

                await Task.Delay(500);
            } while (hasMoreRecords);
        }

        return [.. result
                .DistinctBy(x => new {
                    x.ReferenceId,
                    x.Time
                }).OrderBy(x => x.Time)];
    }

    private async Task<long> GetLastIdAsync(StarRailGachaType gachaType)
    {
        var gachaHistories = repository.Queries.Where(x => x.GachaType == gachaType);
        return !await gachaHistories.AnyAsync() ? 0 : await gachaHistories.MaxAsync(x => x.ReferenceId);
    }

    private async Task<GachaHistoryDataResponse> GetAsync(GetGachaHistoryRequest request, bool scanFirstId = false)
    {
        var response = await hoyoverse.StarRailWishHistoriesAsync(request);
        if (response?.Code == HoyoverseCode.AuthenticateKeyTimeOut)
        {
            throw new BadRequestException("Authenticate key timeout");
        }

        if (response?.Code == HoyoverseCode.VisitTooFrequently)
        {
            return await GetAsync(request);
        }

        if (response?.Data.Items is { Count: <= 0 } && scanFirstId)
        {
            return await GetFirstRemoteIdAsync(request);
        }

        return response?.Data ?? new();
    }

    private async Task<GachaHistoryDataResponse> GetFirstRemoteIdAsync(GetGachaHistoryRequest request)
    {
        var tempBeginId = request.BeginId;
        request.EndId = 0;
        request.BeginId = 0;

        do
        {
            request.Update(request.GachaType, 0, request.EndId);
            var response = await GetAsync(request);
            var items = response.Items;
            if (items is { Count: <= 0 })
            {
                break;
            }

            request.EndId = items.Min(x => x.Id);
        }
        while (true);

        if (request.EndId > tempBeginId)
        {
            request.Update(request.GachaType, request.EndId, 0);
            return await GetAsync(request);
        }

        return new() { Items = [] };
    }
}
