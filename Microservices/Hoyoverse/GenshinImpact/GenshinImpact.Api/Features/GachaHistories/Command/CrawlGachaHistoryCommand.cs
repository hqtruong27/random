namespace GenshinImpact.Api.Features.GachaHistories.Command;

public sealed record CrawlGachaHistoryCommand(string Url) : IRequest<int>;

public class CrawlGachaHistoryCommandHandler(IRepository<GachaHistory> repository
    , IMapper mapper
    , IRepository<Options, string> settingRepository
    , ILogger<CrawlGachaHistoryCommandHandler> logger) : IRequestHandler<CrawlGachaHistoryCommand, int>
{
    public async Task<int> Handle(CrawlGachaHistoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start: crawl {url}", request.Url);

        var queryString = UrlQueryHelper.Populate<UrlQuery>(request.Url);
        var setting = await settingRepository.FirstOrDefaultAsync(x => x.Key == "WISH_LIST_CONFIG");
        var wishListOptions = BsonSerializer.Deserialize<WishListOptions>(setting.Value);

        var gachaHistories = await GetGachaHistoriesAsync(wishListOptions.GachaUrl, queryString);
        if (gachaHistories.Count > 0)
        {
            await repository.BulkInsertAsync(gachaHistories);
        }

        logger.LogInformation("End: crawl success {total}", gachaHistories.Count);

        return gachaHistories.Count;
    }
    private async Task<List<GachaHistory>> GetGachaHistoriesAsync(string gachaUrl, UrlQuery qs)
    {
        List<GachaHistory> result = [];
        foreach (var gachaType in Enum.GetValues<GachaType>().ToList())
        {
            long endId = 0;
            var beginId = await GetLastIdAsync(gachaType);
            bool hasMoreRecords;
            do
            {
                var response = await PostAsync(new(), beginId, endId, gachaUrl, qs, gachaType);
                var gachaHistories = response.GachaHistories;
                switch (gachaHistories.Count)
                {
                    case > 0:
                        result.AddRange(gachaHistories.Select(mapper.Map<GachaHistory>));
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
    private async Task<long> GetLastIdAsync(GachaType gachaType)
    {
        var gachaTypes = new List<GachaType>() { GachaType.CharLimited, GachaType.CharLimitedTwo };
        var gachaHistories =
            gachaTypes.Contains(gachaType)
                ? repository.Queries.Where(x => gachaTypes.Contains(x.GachaType))
                : repository.Queries.Where(x => x.GachaType == gachaType);

        if (!await gachaHistories.AnyAsync())
        {
            return 0;
        }

        return await gachaHistories.MaxAsync(x => x.ReferenceId);
    }
    private static async Task<GachaInfoDataResponse> PostAsync(HttpClient client
       , long beginId
       , long endId
       , string gachaUrl
       , UrlQuery qs
       , GachaType gachaType)
    {
        var requestUri = $"{gachaUrl}?{qs.ToQueryParams((int)gachaType, beginId, endId)}";

        var response = await client.GetAsync(requestUri).ConfigureAwait(false);

        var stream = await response.Content.ReadAsStreamAsync();
        var gachaInfo = await JsonSerializer.DeserializeAsync<GachaInfoResponse>(stream);

        switch (gachaInfo?.Code)
        {
            case HoyolabCode.AuthenticateKeyError:
                throw new Exception("Auth key timeout");
            case HoyolabCode.VisitTooFrequently:
                await PostAsync(client, beginId, endId, gachaUrl, qs, gachaType);
                break;
        }

        return gachaInfo?.Data ?? new();
    }
}