namespace Infrastructure.Persistence.Schemas.Base;

public interface IEntity<out T>
{
    T Id { get; }
}
