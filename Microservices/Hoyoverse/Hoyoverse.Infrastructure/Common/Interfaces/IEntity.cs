namespace Hoyoverse.Infrastructure.Common.Interfaces;

public interface IEntity<out T>
{
    T Id { get; }
}
