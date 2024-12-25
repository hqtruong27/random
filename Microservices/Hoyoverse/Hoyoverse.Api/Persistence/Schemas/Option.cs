namespace Hoyoverse.Persistence.Schemas;

public class Option : AuditableEntity
{
    public string Key { get; set; } = default!;
    public BsonDocument Value { get; set; } = default!;
}
