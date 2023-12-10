using MongoDB.Bson.Serialization.Attributes;

namespace Hoyoverse.Infrastructure.Common.Interfaces;

public interface IAuditableEntity<T> : IEntity<T>
{
    T? CreatedBy { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    DateTime? Created { get; set; }
    T? UpdatedBy { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    DateTime? Updated { get; set; }
}