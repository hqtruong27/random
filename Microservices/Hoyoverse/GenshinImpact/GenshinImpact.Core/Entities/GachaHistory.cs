using GenshinImpact.Core.Base;
using GenshinImpact.Core.EnumTypes;

namespace GenshinImpact.Core.Entities;

public class GachaHistory : AuditableEntity
{
    public long ReferenceId { get; set; }
    public string Uid { get; set; } = default!;
    public GachaType GachaType { get; set; }
    public string ItemId { get; set; } = default!;
    public int Count { get; set; }
    public DateTimeOffset Time { get; set; }
    public string Name { get; set; } = default!;
    public string Lang { get; set; } = default!;
    public ItemType ItemType { get; set; }
    public RankType RankType { get; set; }
}