namespace Hoyoverse.Features.GenshinImpact.Events;

[Event("hello-created")]
public class HelloCreated : IEvent
{
    public string? Description { get; set; }
    public string? Name { get; set; }
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class HelloEventHandler : Reactor<HelloCreated>
{
    public override async Task Handle(HelloCreated @event, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

public class NotHelloEventHandler : Reactor<HelloCreated>
{
    public override async Task Handle(HelloCreated @event, CancellationToken cancellationToken)
    {
        //await DomainEvent.Raise(new DomainHelloCreated
        //{
        //    Description = "Hello",
        //    Name = "World",
        //    Id = Guid.NewGuid()
        //});

        await Task.CompletedTask;
    }
}