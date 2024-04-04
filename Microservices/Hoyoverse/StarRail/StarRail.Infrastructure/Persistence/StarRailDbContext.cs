using MongoDB.Driver;
using StarRail.Core.Entities;
using StarRail.Infrastructure.Common.Settings;

namespace StarRail.Infrastructure.Persistence;
public class StarRailDbContext : IStarRailDbContext
{
    private readonly IDbContextOptions _options;
    private readonly Lazy<IMongoDatabase> _database;
    private IMongoDatabase Database => _database.Value;
    public IMongoCollection<GachaHistory> GachaHistories => Set<GachaHistory>();
    public IMongoCollection<T> Set<T>(string name) => Set<T>(name);
    public IMongoCollection<T> Set<T>() => Database.GetCollection<T>(typeof(T).Name);
    public StarRailDbContext(IDbContextOptions options)
    {
        _options = options;
        _database = new(Connect);
    }
    private IMongoDatabase Connect()
    {
        var client = new MongoClient(_options.ConnectionString);
        return client.GetDatabase(_options.Database);
    }
}