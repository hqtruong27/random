using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Options;

public class DbContextOptionsBuilder(IServiceCollection services)
{
    public IDbContextOptions Options { get; set; } = default!;
    public DbContextOptionsBuilder WithOptions(IConfigurationSection section)
    {
        Options ??= section.Get<MongoDbContextOptions>()!;
        services.Configure<IDbContextOptions>(o => o = Options);
        services.AddSingleton(Options);

        return this;
    }

    public DbContextOptionsBuilder AutoScanMongoConfiguration(Assembly assembly)
    {
        var configTypes = assembly
            .GetTypes()
            .Where(t => typeof(ISchemaConfiguration).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var configType in configTypes)
        {
            var configInstance = Activator.CreateInstance(configType);
            if (configInstance is ISchemaConfiguration instance)
            {
                instance?.Configure();
            }
        }

        return this;
    }
}
