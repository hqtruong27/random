namespace Random.ExternalService.Services;

public interface IStarRailService
{
    [Get("/common/gacha_record/api/getGachaLog")]
    Task<GachaHistoryResponse> GetGachaHistoriesAsync([Query] GetGachaHistoryRequest query);
}
