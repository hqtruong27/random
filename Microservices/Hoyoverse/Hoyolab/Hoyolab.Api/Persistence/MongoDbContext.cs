namespace Hoyolab.Api.Persistence;

public class MongoDbContext(IMongoDatabase database) : IDatabaseContext
{
    public IMongoCollection<User> Users => database.GetCollection<User>(nameof(Users));
    public IMongoCollection<Option> Options => database.GetCollection<Option>(nameof(Options));
    public IMongoCollection<T> Collection<T>(string name) => database.GetCollection<T>(name);
    public IMongoCollection<T> Collection<T>()
    {
        return typeof(T).Name switch
        {
            nameof(User) => database.GetCollection<T>(nameof(Users)),
            nameof(Option) => database.GetCollection<T>(nameof(Options)),
            _ => database.GetCollection<T>($"{typeof(T).Name}s")
        };
    }
}