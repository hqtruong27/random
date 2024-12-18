using Discord.Bot.Features.Tracks.Autocompletes;

namespace Discord.Bot.Features.Musics.Interactions;

public class Play : ApplicationCommandsModule
{
    [SlashCommand("play", "Play a track.")]
    public async Task Handle(InteractionContext ctx, [Autocomplete(typeof(MusicSearchAutocompleteProvider))][Option("query", "The query to search for", true)] string query)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
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

        await guildPlayer.PlayAsync(track);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Now playing {query}!"));

        //if (guildPlayer.Player.PlayerState.Position.TotalSeconds <= 0)
        //{
        //    guildPlayer.PlayQueueAsync();
        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Now playing {query}!"));
        //}
        //else
        //{
        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added to queue {query}!"));
        //}
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