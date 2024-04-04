
namespace Random.ExternalService;

public static class ServiceRegister
{
    public static IServiceCollection AddExternalService(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRefitClient<IStarRailService>()
            .ConfigureHttpClient(c => c.BaseAddress = new(configuration["StarRail:BaseUrl"]!));

        return services;
    }
}
