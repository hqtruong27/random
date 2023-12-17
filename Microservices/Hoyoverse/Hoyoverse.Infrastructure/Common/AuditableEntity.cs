using Hoyoverse.Infrastructure.Common.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Common;

public class AuditableEntity<T> : IAuditableEntity<T>
{
    [BsonId]
    public T Id { get; set; } = default!;
    public T? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public T? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
}

public class AuditableEntity : AuditableEntity<string>
{

}