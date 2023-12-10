using Hoyoverse.Infrastructure.Common;

namespace Hoyoverse.Infrastructure.Entities;

public class Settings : AuditableEntity<string>
{
    public string Key { get; set; } = default!;
}
