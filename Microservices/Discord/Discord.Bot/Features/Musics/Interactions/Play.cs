using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.Entities;
using DisCatSharp.Lavalink.Enums;
using Discord.Bot.Features.Musics.Autocompletes;

namespace Discord.Bot.Features.Musics.Interactions;

public class Play : ApplicationCommandsModule
{
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
        if (loadResult.LoadType is LavalinkLoadResultType.Empty or LavalinkLoadResultType.Error)
        {
            Console.WriteLine(loadResult.LoadType);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Track search failed for {query}."));
            return;
        }

        var track = loadResult.LoadType switch
        {
            LavalinkLoadResultType.Track => loadResult.GetResultAs<LavalinkTrack>(),
            LavalinkLoadResultType.Playlist => loadResult.GetResultAs<LavalinkPlaylist>().Tracks.First(),
            LavalinkLoadResultType.Search => loadResult.GetResultAs<List<LavalinkTrack>>().First(),
            _ => throw new InvalidOperationException("Unexpected load result type.")
        };

        guildPlayer.AddToQueue(new Test(), track);

        if (guildPlayer.Player.PlayerState.Position.TotalSeconds <= 0)
        {
            guildPlayer.PlayQueueAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Now playing {query}!"));
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added to queue {query}!"));
        }
    }

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
}


public class Test : IQueueEntry
{
    public LavalinkTrack Track { get; set; } = default!;

    public Task AfterPlayingAsync(LavalinkGuildPlayer player)
    {
        return Task.FromResult(true);
    }

    public Task<bool> BeforePlayingAsync(LavalinkGuildPlayer player)
    {
        return Task.FromResult(true);
    }
}