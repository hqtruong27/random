namespace Infrastructure.Dispatchers;

public interface IEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}