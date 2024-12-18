using MongoDB.Driver;

namespace Infrastructure.Persistence;

public interface IDbContext
{
    IMongoCollection<T> Set<T>();
    IMongoCollection<T> Set<T>(string name);
}
