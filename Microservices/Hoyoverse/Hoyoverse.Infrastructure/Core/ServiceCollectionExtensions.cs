using GenshinImpact.Persistence.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddHoyoverseDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IHoyoverseDbContext, HoyoverseDbContext>();
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));
        services.AddScoped(typeof(ISettingRepository), typeof(SettingRepository));
    }
}