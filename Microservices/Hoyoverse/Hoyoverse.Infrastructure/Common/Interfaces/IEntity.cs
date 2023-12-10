using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Common.Interfaces;

public interface IEntity<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    T Id { get; }
}
