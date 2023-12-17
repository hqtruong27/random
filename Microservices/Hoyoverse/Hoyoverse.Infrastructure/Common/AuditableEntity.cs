using Hoyoverse.Infrastructure.Common.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hoyoverse.Infrastructure.Common;

public class AuditableEntity<T> : IAuditableEntity<T>
{
    [BsonId]
    public T Id { get; set; } = default!;
    public T? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public T? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
    public AuditableEntity()
    {
        if (typeof(T) == typeof(ObjectId) && Id == null)
        {
            Id = (T)(object)ObjectId.GenerateNewId();
        }
    }
}

public class AuditableEntity : AuditableEntity<string>
{
   
}