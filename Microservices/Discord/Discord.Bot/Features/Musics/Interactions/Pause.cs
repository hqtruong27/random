using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Lavalink;

namespace Discord.Bot.Features.Musics.Interactions;

public class Pause
{
    [SlashCommand("pause", "Pause a track")]
    public static async Task PauseAsync(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(DisCatSharp.Enums.InteractionResponseType.DeferredChannelMessageWithSource);
        if (ctx.Member?.VoiceState?.Channel is null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You are not in a voice channel."));
            return;
        }

        var lavalink = ctx.Client.GetLavalink();
        var guildPlayer = lavalink.GetGuildPlayer(ctx.Guild!);

        if (guildPlayer == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Lavalink is not connected."));
            return;
        }

        if (guildPlayer.CurrentTrack == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("There are no tracks loaded."));
            return;
        }

        await guildPlayer.PauseAsync();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playback paused!"));
    }
}
