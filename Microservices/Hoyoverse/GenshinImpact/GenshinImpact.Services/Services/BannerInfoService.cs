using Common.Helpers;

namespace GenshinImpact.Services.Services;

public class BannerInfoService(IRepository<BannerInfo, string> repository) : IBannerInfoService
{
    private readonly IRepository<BannerInfo, string> _repository = repository;

    public Task CreateAsync(BannerInfoRequest request)
    {
        var query = UrlQueryHelper.Populate<UrlQuery>(request.Link);

        return _repository.InsertAsync(new BannerInfo
        {
            GameVersion = query.GameVersion,
            Region = query.Region,
            GachaId = query.GachaId,
        });
    }
}
