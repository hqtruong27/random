using GenshinImpact.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Hoyoverse.Infrastructure.Core;

public static class ServiceCollectionExtensions
{
    public static void AddHoyoverseDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IHoyoverseDbContext, HoyoverseDbContext>();
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));
        services.AddScoped(typeof(ISettingRepository), typeof(SettingRepository));
    }
}