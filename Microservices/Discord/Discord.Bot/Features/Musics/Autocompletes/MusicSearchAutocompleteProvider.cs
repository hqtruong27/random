using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.Entities;
using DisCatSharp.Lavalink.Enums;

namespace Discord.Bot.Features.Musics.Autocompletes;

public class MusicSearchAutocompleteProvider : IAutocompleteProvider
{
    public async Task<IEnumerable<DiscordApplicationCommandAutocompleteChoice>> Provider(AutocompleteContext context)
    {
        var lavalink = context.Client.GetLavalink();
        var guildPlayer = lavalink.GetGuildPlayer(context.Guild!);
        if (guildPlayer == null)
        {
            return [];
        }

        var query = context.Options[0].Value.ToString() ?? "";
        var loadResult = await guildPlayer.LoadTracksAsync(LavalinkSearchType.Youtube, query);

        if (loadResult.LoadType == LavalinkLoadResultType.Empty || loadResult.LoadType == LavalinkLoadResultType.Error)
        {
            return [];
        }

        var track = (loadResult.LoadType switch
        {
            LavalinkLoadResultType.Track => [loadResult.GetResultAs<LavalinkTrack>()],
            LavalinkLoadResultType.Playlist => loadResult.GetResultAs<LavalinkPlaylist>().Tracks,
            LavalinkLoadResultType.Search => loadResult.GetResultAs<List<LavalinkTrack>>(),
            _ => throw new InvalidOperationException("Unexpected load result type.")
        }).Take(5);

        return track.Select(x => new DiscordApplicationCommandAutocompleteChoice(
            Truncate($"✍🏻 {x.Info.Author} 🎼 {Truncate(x.Info.Title, 50)} 🕑 {FormatTimestamp(x.Info.Length)}", 100),
            Truncate(x.Info.Title, 100)
            )
        );
    }

    private static string FormatTimestamp(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours < 1)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
        else
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }

    private static string Truncate(string value, int length)
    {
        return value.Length > length ? string.Concat(value.AsSpan(1, length - 3), "...") : value;
    }
}
