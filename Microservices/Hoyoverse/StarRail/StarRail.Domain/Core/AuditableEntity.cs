using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StarRail.Domain.Core;

public class AuditableEntity<T> : IAuditableEntity<T>
{
    [BsonId]
    public T Id { get; set; } = default!;
    public string? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public string? UpdatedBy { get; set; }
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