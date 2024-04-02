namespace StarRail.Infrastructure.Common.Settings;

internal interface IDbContextOptions
{
    string CollectionName { get; set; }
    string ConnectionString { get; set; }
    string Database { get; set; }
}

internal class MongoDbContextOptions : IDbContextOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
    public string CollectionName { get; set; } = default!;
}