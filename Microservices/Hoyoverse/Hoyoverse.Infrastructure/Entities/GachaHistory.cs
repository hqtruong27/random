using Common.Enum.Hoyoverse;
using Hoyoverse.Infrastructure.Common;

namespace Hoyoverse.Infrastructure.Entities;

public class GachaHistory : AuditableEntity<long>
{
    public string Uid { get; set; } = default!;
    public GachaType GachaType { get; set; } = default!;
    public string ItemId { get; set; } = default!;
    public int Count { get; set; } = default!;
    public DateTime Time { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Lang { get; set; } = default!;
    public ItemType ItemType { get; set; } = default!;
    public RankType RankType { get; set; } = default!;
}