using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hoyoverse.Infrastructure.Common.Interfaces;

public interface IEntity<T>
{
    T Id { get; }
}
