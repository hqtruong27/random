using Discord.Audio;
using Discord.Interactions;
using NAudio.Wave;
using System.Diagnostics;
using Xabe.FFmpeg;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
namespace Discord.Bot.Features.Musics;

[Group(name: "music", description: "play music")]
public class JoinVoiceChannel : InteractionModuleBase<SocketInteractionContext>
{
    private static IAudioClient audioClient;
    [SlashCommand("join", "join voice channel", runMode: RunMode.Async)]
    public async Task ExecuteAsync()
    {
        var user = Context.User.ToSocketGuild();
        var voiceChannel = user.VoiceChannel;
        if (voiceChannel == null)
        {
            await ReplyAsync("You need to be in a voice channel or specify a voice channel!");
            return;
        }

        audioClient = await voiceChannel.ConnectAsync();
    }


    [SlashCommand("play", "play a song")]
    public async Task PlayAsync(string query)
    {
        if (audioClient == null)
        {
            await ExecuteAsync();
        }

        //var youtube = new YoutubeClient();

        //var video = await youtube.Search.GetVideosAsync(query).FirstOrDefaultAsync();
        //if (video == null)
        //{
        //    await ReplyAsync("No results found on YouTube.");
        //    return;
        //}

        //var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
        //var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        //if (streamInfo == null)
        //{
        //    await ReplyAsync("No suitable audio stream found.");
        //    return;
        //}

        //var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

        //await PlayAudioStream(video.Url);
        query = "1.mp3";
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(query);
        IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault();
       
    }

    private Process CreateFFmpegProcess(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        })!;
    }

    private async Task PlayAudioStream(string audioUrl)
    {
        var httpClient = new System.Net.Http.HttpClient();
        var response = await httpClient.GetAsync(audioUrl, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to download audio stream.");
        }

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var discordStream = audioClient.CreateOpusStream())
        {
            var waveStream = new WaveFileReader(stream);

            await waveStream.CopyToAsync(discordStream);
            await discordStream.FlushAsync().ConfigureAwait(false);
        }
    }

    private async Task SendSingleAsync(string path)
    {
        var format = new WaveFormat(48000, 16, 2);
        using var reader = new MediaFoundationReader(path);
        using var resamplerDmo = new ResamplerDmoStream(reader, format);

        var discordStream = audioClient.CreatePCMStream(AudioApplication.Mixed);
        await resamplerDmo.CopyToAsync(discordStream)
           .ContinueWith(t => { return; });
        await discordStream.FlushAsync();
    }
}
