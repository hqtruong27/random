
namespace Hoyoverse.Features.Hoyolab.Events;

public class RedeemCodeRedeemed : IEvent
{
    public Guid Id => Guid.NewGuid();

    public DateTime OccurredOn => DateTime.UtcNow;
    public List<RedeemCodeMessage> Redeems { get; set; } = default!;
}

public class RedeemCodeMessage
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
}