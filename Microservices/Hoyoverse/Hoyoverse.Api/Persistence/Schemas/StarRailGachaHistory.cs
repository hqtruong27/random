namespace Hoyoverse.Persistence.Schemas;

public class StarRailGachaHistory : AuditableEntity<ObjectId>
{
    public string UserId { get; private init; } = default!;
    public long ReferenceId { get; private init; }
    public string Uid { get; private init; } = default!;
    public StarRailGachaType GachaType { get; private set; }
    public string ItemId { get; private init; } = default!;
    public int Count { get; private set; }
    public DateTimeOffset Time { get; private init; }
    public string Name { get; private set; } = default!;
    public string Lang { get; private set; } = default!;
    public StarRailItemType ItemType { get; private set; }
    public StarRailRankType RankType { get; private set; }
}