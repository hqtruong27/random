using MongoDB.Bson;
using Hoyoverse.Infrastructure.Common;

namespace Hoyoverse.Infrastructure.Entities;

public class AuthToken : AuditableEntity<ObjectId>
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
}
