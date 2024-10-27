namespace Hoyolab.Api.Persistence.Entities.Base;

public interface IEntity<out T>
{
    T Id { get; }
}
