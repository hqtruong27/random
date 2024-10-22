using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Entities;

public class User : AuditableEntity<ObjectId>
{
    public required DiscordAccount Discord { get; set; }
    public required ICollection<HoyolabAccount> Hoyolabs { get; set; }
}

public class HoyolabAccount
{
    public required string Id { get; set; }
    public required string Cookie { get; set; }
    public string Username { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public bool IsAutoCheckIn { get; set; }
    public List<HoyolabGame> Games { get; set; } = [];
}

public class DiscordAccount
{
    public required string Id { get; set; } = default!;
    public required string Username { get; set; } = default!;
}

public enum HoyolabGame
{
    GenshinImpact,
    StarRail,
    HonkaiImpact3,
    ZenlessZoneZero
}