namespace Hoyoverse.Features.GenshinImpact.GachaHistories.Queries;

[Tags("GenshinImpact")]
[Get("genshin-impact/get-gacha-histories")]
public record GetGachaHistoriesQuery : IRequest<List<GachaHistoryResponse>>;

public class GetGachaHistoriesQueryHandler(IRepository<GenshinImpactGachaHistory, string> repository, IMapper mapper) :
    IRequestHandler<GetGachaHistoriesQuery, List<GachaHistoryResponse>>
{
    public async Task<List<GachaHistoryResponse>> Handle(GetGachaHistoriesQuery request, CancellationToken cancellationToken)
    {
        var result = repository.Queries.OrderByDescending(x => x.Time).ToList();
        return await Task.FromResult(result.Select(mapper.Map<GachaHistoryResponse>).ToList());
    }
}