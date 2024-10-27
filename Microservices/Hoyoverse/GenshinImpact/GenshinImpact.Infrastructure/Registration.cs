using GenshinImpact.Core.Interfaces.Repositories;
using GenshinImpact.Infrastructure.Persistence.Configurations;
using GenshinImpact.Infrastructure.Persistence.Repositories;

namespace GenshinImpact.Infrastructure;

public static class Registration
{
    public static IServiceCollection AddGenshinImpactDbContext(this IServiceCollection services, IDbContextOptions options)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddSingleton(new MongoClient(options.ConnectionString).GetDatabase(options.Database));
        services.AddScoped<MongoDbContext>();
        services.AddScoped<IDatabaseContext, MongoDbContext>();

        AddEntityTypeConfiguration();
        return services.AddRepositories();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

        return services;
    }

    public static void AddEntityTypeConfiguration()
    {
        GachaHistoryConfiguration.Configure();
    }
}