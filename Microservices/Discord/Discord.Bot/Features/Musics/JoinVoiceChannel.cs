using Discord.Audio;
using Discord.Interactions;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
namespace Discord.Bot.Features.Musics;

[Interactions.Group(name: "music", description: "play music")]
public class JoinVoiceChannel : InteractionModuleBase<SocketInteractionContext>
{
    private static IAudioClient? _audioClient;

    [SlashCommand("join", "join voice channel", runMode: Interactions.RunMode.Async)]
    public async Task JoinAsync()
    {
        var user = Context.User.ToSocketGuild();
        var voiceChannel = user.VoiceChannel;
        if (voiceChannel == null)
        {
            await ReplyAsync("You need to be in a voice channel or specify a voice channel!");
            return;
        }

        _audioClient = await voiceChannel.ConnectAsync();

        return;
    }

    [SlashCommand("ping", "a", runMode: Interactions.RunMode.Async)]
    public async Task Ping()
    {
        await RespondAsync("Pong!");
    }

    [SlashCommand("play", "play a song")]
    public async Task PlayAsync(string query)
    {
        _ = Task.Run(async () =>
        {
            await JoinAsync();

            var youtube = new YoutubeClient();

            var token = new CancellationTokenSource().Token;
            var video = await youtube.Search.GetVideosAsync(query, token).FirstOrDefaultAsync(token);
            if (video == null)
            {
                await ReplyAsync("No results found on YouTube.");
                return;
            }

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id, token);
            var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

            if (streamInfo == null)
            {
                await ReplyAsync("No suitable audio stream found.");
                return;
            }

            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Content = video.Title);
            using var discordStream = _audioClient!.CreatePCMStream(AudioApplication.Mixed, bufferMillis: 1024 * 16);
            try
            {
                using var ffmpeg = CreateStream(streamInfo.Url);
                //var ffmpegErrors = await ffmpeg.StandardError.ReadToEndAsync();
                //if (!string.IsNullOrEmpty(ffmpegErrors))
                //{
                //    Console.WriteLine($"FFmpeg error: {ffmpegErrors}");
                //}
                //// Rest of your code
                await ffmpeg.StandardOutput.BaseStream
                            .CopyToAsync(discordStream, bufferSize: 1024 * 16, token)
                            .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during streaming: {ex.Message}", token);
            }
            finally
            {
                await discordStream.FlushAsync(token);
            }
        });

        await RespondAsync("querying...");
    }

    private static Process CreateStream(string url)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{url}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        })!;
    }

    private static async Task<string> GetYouTubeAudioUrlAsync(string query)
    {
        // Start yt-dlp process to fetch the audio URL
        var ytDlpProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-f bestaudio --get-url \"ytsearch1:{query}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        ytDlpProcess.Start();

        var url = await ytDlpProcess.StandardOutput.ReadToEndAsync();

        await ytDlpProcess.WaitForExitAsync();
        ytDlpProcess.Dispose();

        return url.Trim();
    }
}
