namespace Discord.Bot.Features.Musics.Commands;

public class Join : BaseCommandModule
{
    [Command("join")]
    public static async Task JoinAsync(CommandContext ctx)
    {
        var lavalink = ctx.Client.GetLavalink();
        var session = lavalink.ConnectedSessions.Values.First();
        var channel = ctx.Member.VoiceState?.Channel;
        if (channel is null)
        {
            await ctx.RespondAsync("Not in channel.");
            return;
        }

        if (channel.Type is not (DisCatSharp.Enums.ChannelType.Voice or DisCatSharp.Enums.ChannelType.Stage))
        {
            await ctx.RespondAsync("Not a valid voice channel.");
            return;
        }

        await session.ConnectAsync(channel);
        await ctx.RespondAsync($"Joined {channel.Mention}!");
    }
}
