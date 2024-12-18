using Hoyoverse.Persistence.Schemas.Base;

namespace Hoyoverse.Persistence.Schemas;

public class Option : AuditableEntity<string>
{
    public string Key { get; set; } = default!;
    public BsonDocument Value { get; set; } = default!;
}
