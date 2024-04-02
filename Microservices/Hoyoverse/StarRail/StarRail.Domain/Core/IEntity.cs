namespace StarRail.Domain.Core;

public interface IEntity<T>
{
    T Id { get; }
}
