﻿namespace Hoyolab.Api.Persistence;

public class MongoDbContext(IMongoDatabase database) : IDatabaseContext
{
    public IMongoCollection<User> Users => database.GetCollection<User>(nameof(User));
    public IMongoCollection<Option> Options => database.GetCollection<Option>(nameof(Options));
    public IMongoCollection<T> Collection<T>() => database.GetCollection<T>(typeof(T).Name);
    public IMongoCollection<T> Collection<T>(string name) => database.GetCollection<T>(name);
}