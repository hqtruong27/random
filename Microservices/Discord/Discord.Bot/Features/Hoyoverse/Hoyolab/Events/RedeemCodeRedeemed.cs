using Discord.Bot.Features.Hoyoverse.Hoyolab.Contracts;
using Discord.Shared.Helpers;
using Infrastructure.Dispatchers;

namespace Discord.Bot.Features.Hoyoverse.Hoyolab.Events;

public class RedeemCodeRedeemed : IEvent
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public DiscordInformation Discord { get; set; } = default!;
    public List<RedeemCodeMessage> Redeems { get; set; } = default!;
}

public class RedeemCodeRedeemedHandler(DiscordClient discord, ILogger<RedeemCodeRedeemedHandler> logger) : Reactor<RedeemCodeRedeemed>
{
    public override async Task Handle(RedeemCodeRedeemed @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Redeem code redeemed event: {event}", @event);

        var guild = await discord.GetGuildAsync(@event.Discord.GuildId);

        var channel = guild.GetChannel(@event.Discord.ChannelId);

        var description = @event.Redeems.CreateTable();

        var embed = new DiscordEmbedBuilder()
           .WithTitle($"Redemption code: {@event.Discord.Game}")
           .WithColor(DiscordColor.Gold)
           .WithDescription(description)
           .Build();

        await channel.SendMessageAsync(embed);
    }
}