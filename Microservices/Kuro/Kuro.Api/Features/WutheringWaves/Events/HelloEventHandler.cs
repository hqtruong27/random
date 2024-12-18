namespace Kuro.Features.WutheringWaves.Events;

[Event("hello-created")]
public class HelloCreated : IEvent
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class HelloEventHandler : IEventHandler<HelloCreated>
{
    public async Task Handle(HelloCreated @event, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

public class NotHelloEventHandler : IEventHandler<HelloCreated>
{
    public async Task Handle(HelloCreated @event, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}