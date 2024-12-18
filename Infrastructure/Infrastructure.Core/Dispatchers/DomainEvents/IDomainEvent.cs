namespace Infrastructure.Dispatchers;

public interface IDomainEvent : INotification
{
    Guid Id { get; set; }
}