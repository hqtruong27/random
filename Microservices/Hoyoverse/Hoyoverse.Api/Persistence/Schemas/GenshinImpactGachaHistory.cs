namespace Hoyoverse.Persistence.Schemas;

public class GenshinImpactGachaHistory : AuditableEntity
{
    public long ReferenceId { get; set; }
    public string Uid { get; set; } = default!;
    public GenshinImpactGachaType GachaType { get; set; }
    public string ItemId { get; set; } = default!;
    public int Count { get; set; }
    public DateTimeOffset Time { get; set; }
    public string Name { get; set; } = default!;
    public string Lang { get; set; } = default!;
    public GenshinImpactItemType ItemType { get; set; }
    public GenshinImpactRankType RankType { get; set; }
}