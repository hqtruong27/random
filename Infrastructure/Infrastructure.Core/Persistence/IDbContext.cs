using MongoDB.Driver;

namespace Infrastructure.Persistence;

public interface IDbContext
{
    IQueryable<T> Queries<T>();
    IMongoCollection<T> Set<T>();
    IMongoCollection<T> Set<T>(string name);
}
