using Infrastructure.Dispatchers;

namespace Discord.Bot.Features.Hoyoverse.Hoyolab.Events;

public class RedeemCodeMessage
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
}

public class RedeemCodeRedeemed : IEvent
{
    public Guid Id { get; set; }

    public DateTime OccurredOn { get; set; }

    public List<RedeemCodeMessage> Redeems { get; set; } = default!;
}

public class RedeemCodeRedeemedHandler(ILogger<RedeemCodeRedeemedHandler> logger) : Reactor<RedeemCodeRedeemed>
{
    public override Task Handle(RedeemCodeRedeemed @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Redeem code redeemed event: {event}", @event);
        return Task.CompletedTask;
    }
}