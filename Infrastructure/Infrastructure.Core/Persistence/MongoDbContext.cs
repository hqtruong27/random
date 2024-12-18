using Humanizer;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class MongoDbContext : IDbContext
{
    protected readonly IDbContextOptions _options;
    protected readonly Lazy<IMongoDatabase> _database;
    protected IMongoDatabase Database => _database.Value;
    public IMongoCollection<T> Set<T>(string name) => Database.GetCollection<T>(name);
    public IMongoCollection<T> Set<T>() => Database.GetCollection<T>(typeof(T).Name.Underscore());
    public MongoDbContext(IDbContextOptions options)
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