namespace Discord.Bot.Features.Hoyoverse.Hoyolab.Contracts;

public class RedeemCodeMessage
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
}

public class DiscordInformation
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string Game { get; set; } = default!;
}