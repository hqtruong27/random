namespace Infrastructure.Persistence.Options;

public interface IDbContextOptions
{
    string CollectionName { get; set; }
    string ConnectionString { get; set; }
    string Database { get; set; }
}

public class MongoDbContextOptions : IDbContextOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
    public string CollectionName { get; set; } = default!;
}