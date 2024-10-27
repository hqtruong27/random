using Hoyolab.Api.Persistence.Entities.Base;

namespace Hoyolab.Api.Persistence.Entities;

public class User : AuditableEntity
{
    public DiscordAccount Discord { get; set; } = default!;
    public ICollection<HoyolabAccount> Hoyolabs { get; set; } = default!;
}
public class HoyolabAccount
{
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }
    public required string Cookie { get; set; }
    public string Username { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public bool IsAutoCheckIn { get; set; }
    public List<HoyolabGame> Games { get; set; } = [];
}

public class DiscordAccount
{
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }
    public required string Username { get; set; }
}

public enum HoyolabGame
{
    GenshinImpact,
    StarRail,
    HonkaiImpact3,
    ZenlessZoneZero
}