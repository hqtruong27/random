using Hoyoverse.Infrastructure.Common.Settings;
using MongoDB.Driver;

namespace Hoyoverse.Infrastructure;

public interface IHoyoverseDbContext
{
    IMongoDatabase Database { get; }
}

public class HoyoverseDbContext : IHoyoverseDbContext
{
    private readonly IDbContextOptions _options;
    private readonly Lazy<IMongoDatabase> _database;
    public HoyoverseDbContext(IDbContextOptions options)
    {
        _options = options;
        _database = new(Connect);
    }
    public IMongoDatabase Database => _database.Value;

    private IMongoDatabase Connect()
    {
        var client = new MongoClient(_options.ConnectionString);
        return client.GetDatabase(_options.Database);
    }
}