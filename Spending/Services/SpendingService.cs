using Grpc.Net.Client;
using WebApi.Protos;

namespace Spending.Services;
public interface ISpendingService
{
    Task<GetSpendingResponse> GetAsync(GetRequest request);
}

public class SpendingService : SpendingClient, ISpendingService
{
    private readonly SpendingClient _client;
    public SpendingService()
    {
        _client = new(GrpcChannel.ForAddress("https://localhost:7072"));
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