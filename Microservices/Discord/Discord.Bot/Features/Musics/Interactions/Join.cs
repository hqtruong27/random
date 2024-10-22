namespace Discord.Bot.Features.Musics.Interactions;

public class Join : ApplicationCommandsModule
{
    [SlashCommand("join", "join voice channel.")]
    public static async Task Handle(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(DisCatSharp.Enums.InteractionResponseType.DeferredChannelMessageWithSource);
        var lavalink = ctx.Client.GetLavalink();
        var session = lavalink.ConnectedSessions.Values.First();
        var channel = ctx.Member!.VoiceState?.Channel;
        if (channel is null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Not in channel."));
            return;
        }

        if (channel.Type is not (DisCatSharp.Enums.ChannelType.Voice or DisCatSharp.Enums.ChannelType.Stage))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Not a valid voice channel."));
            return;
        }

        await session.ConnectAsync(channel);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Joined {channel.Mention}!"));
    }
}