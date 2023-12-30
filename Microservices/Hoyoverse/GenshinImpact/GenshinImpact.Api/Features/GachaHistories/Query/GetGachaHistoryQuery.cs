using AutoMapper;

namespace GenshinImpact.Api.Features.GachaHistories.Query;

public sealed record GetGachaHistoryQuery(long Id) : IRequest<GachaHistoryResponse>
{
    public class GetGachaHistoryByIdQueryHandler(IRepository<GachaHistory, long> repository, IMapper mapper) :
        IRequestHandler<GetGachaHistoryQuery, GachaHistoryResponse>
    {
        public async Task<GachaHistoryResponse> Handle(GetGachaHistoryQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.FindByIdAsync(request.Id);
            return mapper.Map<GachaHistoryResponse>(result);
        }
    }
}