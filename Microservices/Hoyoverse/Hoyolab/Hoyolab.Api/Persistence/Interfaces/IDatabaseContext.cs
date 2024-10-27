namespace Hoyolab.Api.Persistence.Interfaces;

public interface IDatabaseContext
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<Setting> Settings { get; }
    IMongoCollection<T> Collection<T>();
    IMongoCollection<T> Collection<T>(string name);
}