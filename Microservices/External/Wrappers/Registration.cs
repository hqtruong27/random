using Wrappers.Shared.Extensions;

namespace Wrappers;

public static class Registration
{
    public static IServiceCollection AddExternalService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<DynamicBaseAddressHandler>();

        services
            .AddRefitClient<IHoyoverseService>()
            .ConfigureHttpClient(client => client.BaseAddress = new(configuration["Hoyoverse:StarRail:BaseUrl"]!))
            .AddHttpMessageHandler<DynamicBaseAddressHandler>();

        services
            .AddRefitClient<IKuroService>()
            .ConfigureHttpClient(c => c.BaseAddress = new(configuration["Hoyoverse:Kuro:BaseUrl"]!));

        return services;
    }
}
