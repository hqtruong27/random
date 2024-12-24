namespace Infrastructure.Persistence.Mongo.Schemas;

public class Entity<T> : IEntity<T>
{
    public T Id { get; set; } = default!;
    public bool IsTransient() => Id != null && Id.Equals(default(T));
}