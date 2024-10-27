using GenshinImpact.Core.Base;

namespace GenshinImpact.Core.Entities;

public class BannerInfo : AuditableEntity<string>
{
    public string Region { get; set; } = default!;
    public string GameVersion { get; set; } = default!;
}
