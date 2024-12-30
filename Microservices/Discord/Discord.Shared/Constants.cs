using System.Security.Cryptography;

namespace Discord.Shared;

public class Constants
{
    public static List<string> Playing => [
        "Genshin Impact",
        "Honkai: Star Rail",
        "Wuthering Waves",
        "League of Legends"
    ];

    public static List<string> Watching => ["League of Legends", "Grand Theft Auto VI", "Ananta"];

    public static List<string> Streaming => ["Grand Theft Auto V", "Black Myth: Wukong"];

    public static (string Type, string Status, string Name) GetRandomShuffledActivity()
    {
        var playingList = Playing.Select(item => (nameof(Playing), "Online", item)).ToList();
        var watchingList = Watching.Select(item => (nameof(Watching), "Idle", item)).ToList();
        var streamingList = Streaming.Select(item => (nameof(Streaming), "DoNotDisturb", item)).ToList();

        List<(string, string, string)> combinedList = [
            ..playingList,
            ..watchingList,
            ..streamingList
        ];

        return combinedList.OrderBy(x => GetRandomInt32()).First();
    }

    private static int GetRandomInt32()
    {
        using var rng = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        return BitConverter.ToInt32(randomBytes, 0);
    }
}