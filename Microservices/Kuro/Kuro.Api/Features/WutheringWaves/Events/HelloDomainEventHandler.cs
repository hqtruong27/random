namespace Kuro.Features.WutheringWaves.Events;

public class DomainHelloCreated : IDomainEvent
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public Guid Id { get; set; }
}

public class HelloDomainEventHandler : DomainEventHandler<DomainHelloCreated>
{
    public override async Task Handle(DomainHelloCreated @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{@event.Description}, {@event.Name}");
        await Task.Delay(2000 * 10, cancellationToken);
        Console.WriteLine("Task run successfully");
    }
}
