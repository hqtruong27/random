
namespace GenshinImpact.Infrastructure.Persistence;

public class MongoDbContext(IMongoDatabase database) : IDatabaseContext
{
    public IMongoCollection<GachaHistory> GachaHistories => database.GetCollection<GachaHistory>(nameof(GachaHistories));
    public IMongoCollection<Option> Options => database.GetCollection<Option>(nameof(Options));
    public IMongoCollection<T> Collection<T>()
    {
        return typeof(T).Name switch
        {
            nameof(GachaHistory) => database.GetCollection<T>(nameof(GachaHistories)),
            nameof(Option) => database.GetCollection<T>(nameof(Options)),
            _ => database.GetCollection<T>($"{typeof(T).Name}s")
        };
    }
    public IMongoCollection<T> Collection<T>(string name) => database.GetCollection<T>(name);
}