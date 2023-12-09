using Grpc.Net.Client;

namespace Discord.Bot.Services;
public interface ISpendingService
{
    Task<GetSpendingResponse> GetAsync(GetRequest request);
}

public class SpendingService : SpendingClient, ISpendingService
{
    private readonly SpendingClient _client;
    public SpendingService(StatisticSettings settings)
    {
        _client = new(GrpcChannel.ForAddress(settings.Gateway));
    }

    public async Task<GetSpendingResponse> GetAsync(GetRequest request)
    {
        return await _client.GetAsync(request);
    }

    public async Task<GetSpendingsByUserIdResponse> GetSpendingsByUserIdAsync(GetSpendingsByUserIdRequest request)
    {
        return await _client.GetSpendingsByUserIdAsync(request);
    }
}