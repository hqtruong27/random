namespace Hoyoverse.Features.GenshinImpact.GachaHistories.Command;

[Tags("GenshinImpact")]
[Post("genshin-impact/crawl-gacha-history")]
public sealed record CrawlGachaHistoryCommand(string Url) : IRequest<int>;

public class CrawlGachaHistoryCommandHandler(HoyoverseDbContext context
    , IMapper mapper
    , IHoyoverseService hoyoverse
    , ILogger<CrawlGachaHistoryCommandHandler> logger) : IRequestHandler<CrawlGachaHistoryCommand, int>
{
    public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start: crawl {url}", request.Url);

        var queryString = UrlQueryHelper.Populate<GetGachaHistoryRequest>(request.Url);
        var gachaHistories = await GetGachaHistoriesAsync(queryString);
        if (gachaHistories.Count > 0)
        {
            await context.GenshinImpactGachaHistories.BulkInsertAsync(gachaHistories, cancellationToken);
        }

        logger.LogInformation("End: crawl success {total}", gachaHistories.Count);

        return gachaHistories.Count;
    }

    private async Task<List<GenshinImpactGachaHistory>> GetGachaHistoriesAsync(GetGachaHistoryRequest request)
    {
        List<GenshinImpactGachaHistory> result = [];
        foreach (var gachaType in Enum.GetValues<GenshinImpactGachaType>().ToList())
        {
            long endId = 0;
            var beginId = await GetLastIdAsync(gachaType);
            bool hasMoreRecords;
            do
            {
                request.Update((int)gachaType, beginId, endId);
                var gachaHistories = (await GetAsync(request)).Items;
                switch (gachaHistories.Count)
                {
                    case > 0:
                        result.AddRange(gachaHistories.Select(mapper.Map<GenshinImpactGachaHistory>));
                        if (beginId == 0)
                        {
                            endId = gachaHistories[^1].Id;
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

        return [.. result
                .DistinctBy(x => new {
                    x.ReferenceId,
                    x.Time
                }).OrderBy(x => x.Time)];
    }

    private async Task<long> GetLastIdAsync(GenshinImpactGachaType gachaType)
    {
        List<GenshinImpactGachaType> gachaTypes = [
            GenshinImpactGachaType.CharLimited,
            GenshinImpactGachaType.CharLimitedTwo
        ];

        var gachaHistories =
            gachaTypes.Contains(gachaType)
                ? context.GenshinImpactGachaHistories.AsQueryable().Where(x => gachaTypes.Contains(x.GachaType))
                : context.GenshinImpactGachaHistories.AsQueryable().Where(x => x.GachaType == gachaType);

        var e = gachaHistories.ToList();
        if (!await gachaHistories.AnyAsync())
        {
            return 0;
        }

        var ea =  gachaHistories.OrderByDescending(x => x.ReferenceId).First();
        return await gachaHistories.MaxAsync(x => x.ReferenceId);
    }

    private async Task<GachaHistoryDataResponse> GetAsync(GetGachaHistoryRequest request)
    {
        var response = await hoyoverse.GenshinImpactWishHistoriesAsync(request);
        if (response?.Code == HoyoverseCode.AuthenticateKeyTimeOut)
        {
            throw new BadRequestException("Authenticate key timeout");
        }

        if (response?.Code == HoyoverseCode.VisitTooFrequently)
        {
            await GetAsync(request);
        }

        return response?.Data ?? new();
    }
}