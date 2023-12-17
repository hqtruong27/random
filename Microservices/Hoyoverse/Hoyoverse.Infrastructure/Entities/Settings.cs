using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Entities;

public class Settings : AuditableEntity<string>
{
    public string Key { get; set; } = default!;
    public BsonDocument Value { get; set; } = default!;
}
