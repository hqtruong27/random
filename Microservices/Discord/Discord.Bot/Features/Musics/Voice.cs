using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.Entities;
using DisCatSharp.Lavalink.Enums;
using Discord.Bot.Features.Musics.Autocompletes;

namespace Discord.Bot.Features.Musics;

[SlashCommandGroup("music", "This is description of the command music group.")]
public class Voice : ApplicationCommandsModule
{
    [SlashCommand("join", "join voice channel.")]
    public static async Task JoinAsync(InteractionContext ctx)
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

    [SlashCommand("play", "Play a track.")]
    public static async Task PlayAsync(InteractionContext ctx, [Autocomplete(typeof(MusicSearchAutocompleteProvider))][Option("query", "The query to search for", true)] string query)
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

        var loadResult = await guildPlayer.LoadTracksAsync(LavalinkSearchType.Youtube, query);

        if (loadResult.LoadType == LavalinkLoadResultType.Empty || loadResult.LoadType == LavalinkLoadResultType.Error)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Track search failed for {query}."));
            return;
        }

        LavalinkTrack track = loadResult.LoadType switch
        {
            LavalinkLoadResultType.Track => loadResult.GetResultAs<LavalinkTrack>(),
            LavalinkLoadResultType.Playlist => loadResult.GetResultAs<LavalinkPlaylist>().Tracks.First(),
            LavalinkLoadResultType.Search => loadResult.GetResultAs<List<LavalinkTrack>>().First(),
            _ => throw new InvalidOperationException("Unexpected load result type.")
        };

        await guildPlayer.PlayAsync(track);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Now playing {query}!"));
    }

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

    [SlashCommand("resume", "Resume a track")]
    public static async Task ResumeAsync(InteractionContext ctx)
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

        await guildPlayer.ResumeAsync();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Playback resumed!"));
    }
}
