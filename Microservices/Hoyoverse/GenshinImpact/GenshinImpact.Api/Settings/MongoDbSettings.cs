namespace GenshinImpact.Api.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
    public string ConnectionName { get; set; } = default!;
}
