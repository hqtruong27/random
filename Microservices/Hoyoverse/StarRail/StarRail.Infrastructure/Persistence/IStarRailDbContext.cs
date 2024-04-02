using MongoDB.Driver;

namespace StarRail.Infrastructure.Persistence;
public interface IStarRailDbContext
{
    public IMongoCollection<T> Set<T>();
    public IMongoCollection<T> Set<T>(string name);
}
