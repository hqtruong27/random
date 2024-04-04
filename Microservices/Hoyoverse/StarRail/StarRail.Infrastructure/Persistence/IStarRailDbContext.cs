using MongoDB.Driver;

namespace StarRail.Infrastructure.Persistence;
public interface IStarRailDbContext
{
    IMongoCollection<T> Set<T>();
    IMongoCollection<T> Set<T>(string name);
}
