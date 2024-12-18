namespace Wrappers.Services;

public interface IHoyoverseService
{
    [Headers(Hoyoverse.Act.GenshinImpact)]
    [Get("/gacha_info/api/getGachaLog")]
    Task<GachaHistoryResponse> GenshinImpactWishHistoriesAsync([Query] GetGachaHistoryRequest query);

    [Headers(Hoyoverse.Act.Hsr)]
    [Get("/common/gacha_record/api/getGachaLog")]
    Task<GachaHistoryResponse> StarRailWishHistoriesAsync([Query] GetGachaHistoryRequest query);
}
