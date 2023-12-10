using GenshinImpact.Persistence.Repositories;
using Hoyoverse.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Hoyoverse.Infrastructure.Core;

public static class ServiceCollectionExtensions
{
    public static void AddHoyoverseDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IHoyoverseDbContext, HoyoverseDbContext>();
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));
        //services.AddScoped(typeof(IBulkRepository<>), typeof(MongoRepository<,>));
    }
}

//public class DbContextOptionsBuilder
//{
//    private readonly IDbContextOptions _options;
//    public DbContextOptionsBuilder(IDbContextOptions options)
//    {
//        _options = options;
//    }
//}

//public class MongoDbContextOptionsBuilder : DbContextOptionsBuilder
//{
//    public MongoDbContextOptionsBuilder(IDbContextOptions options) : base(options)
//    {
//    }
//}

//public static class MongoDbContextOptionsBuilderExtensions
//{
//    public static DbContextOptionsBuilder UseMongo(this DbContextOptionsBuilder optionsBuilder
//        , string? connectionString
//        , Action<MongoDbContextOptionsBuilder>? mongoOptionsAction = null)
//    {
//        return optionsBuilder;
//    }
//}
