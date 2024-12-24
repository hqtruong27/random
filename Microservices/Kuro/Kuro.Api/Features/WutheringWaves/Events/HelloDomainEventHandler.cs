namespace Kuro.Features.WutheringWaves.Events;

public class DomainHelloCreated : IDomainEvent
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public Guid Id { get; set; }
}

public class HelloDomainEventHandler(IRepository<ConveneRecord> repository) : DomainEventHandler<DomainHelloCreated>
{
    public override async Task Handle(DomainHelloCreated @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{@event.Description}, {@event.Name}");
        var convene = await repository.Queries.FirstOrDefaultAsync(cancellationToken);
        Console.WriteLine("Task run successfully");
    }
}
