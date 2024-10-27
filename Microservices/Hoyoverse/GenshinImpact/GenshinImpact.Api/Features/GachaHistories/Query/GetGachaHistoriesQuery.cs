namespace GenshinImpact.Api.Features.GachaHistories.Query;

public record GetGachaHistoriesQuery : IRequest<List<GachaHistoryResponse>>;

public class GetGachaHistoriesQueryHandler(IRepository<GachaHistory, string> repository, IMapper mapper) :
    IRequestHandler<GetGachaHistoriesQuery, List<GachaHistoryResponse>>
{
    public async Task<List<GachaHistoryResponse>> Handle(GetGachaHistoriesQuery request, CancellationToken cancellationToken)
    {
        var result = repository.Queries.OrderByDescending(x => x.Time).ToList();
        return await Task.FromResult(result.Select(mapper.Map<GachaHistoryResponse>).ToList());
    }
}