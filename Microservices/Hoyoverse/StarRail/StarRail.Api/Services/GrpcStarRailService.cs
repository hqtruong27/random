namespace StarRail.Api.Services;

public class GrpcStarRailService(ISender sender) : StarRailService.StarRailServiceBase
{
    public override async Task<WishHistoryResponse> WishHistory(WishHistoryRequest request, ServerCallContext context)
    {
        var response = await sender.Send(new WishCalculatorQuery());
        var result = response.Select(ProfileMapper.Mapper.Map<WishHistory>);

        return new WishHistoryResponse
        {
            WishHistories = { result }
        };
    }
}
