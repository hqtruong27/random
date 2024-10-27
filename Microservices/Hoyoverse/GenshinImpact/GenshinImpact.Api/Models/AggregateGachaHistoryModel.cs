using MongoDB.Bson.Serialization.Attributes;

namespace GenshinImpact.Api.Models;

[BsonIgnoreExtraElements]
public class AggregateGachaHistoryModel
{
    public long ReferenceId { get; set; }
    public int PullIndex { get; set; }
    public string Name { get; set; } = default!;
}