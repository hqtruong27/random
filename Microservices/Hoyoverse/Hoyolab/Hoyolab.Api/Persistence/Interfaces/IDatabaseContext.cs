namespace Hoyolab.Api.Persistence.Interfaces;

public interface IDatabaseContext
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<Option> Options { get; }
    IMongoCollection<T> Collection<T>();
    IMongoCollection<T> Collection<T>(string name);
}