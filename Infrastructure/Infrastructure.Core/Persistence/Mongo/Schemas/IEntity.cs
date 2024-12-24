namespace Infrastructure.Persistence.Mongo.Schemas;

public interface IEntity<out T>
{
    T Id { get; }
}
