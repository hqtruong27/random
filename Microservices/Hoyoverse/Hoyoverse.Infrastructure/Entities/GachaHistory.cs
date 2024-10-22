using Common.Enum.Hoyoverse;
using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Entities;

public class GachaHistory : AuditableEntity<ObjectId>
{
    public long ReferenceId { get; set; }
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