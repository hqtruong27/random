using MongoDB.Bson;
using StarRail.Domain.Core;
using StarRail.Domain.Enums;
namespace StarRail.Domain.Entities;

public class GachaHistory : AuditableEntity<ObjectId>
{
    public string UserId { get; private init; } = default!;
    public long ReferenceId { get; private init; }
    public string Uid { get; private init; } = default!;
    public GachaType GachaType { get; private set; } = default!;
    public string ItemId { get; private init; } = default!;
    public int Count { get; private set; } = default!;
    public DateTime Time { get; private init; } = default!;
    public string Name { get; private set; } = default!;
    public string Lang { get; private set; } = default!;
    public ItemType ItemType { get; private set; } = default!;
    public RankType RankType { get; private set; } = default!;
}