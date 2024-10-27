using GenshinImpact.Core.Base;
using MongoDB.Bson;

namespace GenshinImpact.Core.Entities;

public class Option : AuditableEntity<string>
{
    public string Key { get; set; } = default!;
    public BsonDocument Value { get; set; } = default!;
}
