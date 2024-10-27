namespace GenshinImpact.Core.Base;

public interface IEntity<out T>
{
    T Id { get; }
}
