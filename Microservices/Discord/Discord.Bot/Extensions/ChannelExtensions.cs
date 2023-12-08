namespace Discord.Extensions;

public static class ChannelExtensions
{
    public static async Task<T?> GetChannelAsync<T>(this DiscordSocketClient client, ulong id) where T : class
    {
        return (await client.GetChannelAsync(id)) as T;
    }
}
