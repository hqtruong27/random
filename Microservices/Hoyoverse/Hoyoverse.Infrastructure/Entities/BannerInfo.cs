namespace Hoyoverse.Infrastructure.Entities;

public class BannerInfo : AuditableEntity<string>
{
    public string GameVersion { get; set; } = default!;
    public string Region { get; set; } = default!;
}
