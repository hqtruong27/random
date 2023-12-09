namespace Discord.Bot.Extensions;

public static class GuildExtensions
{
    public static SocketGuildUser ToSocketGuild(this SocketUser user)
    {
        return (user as SocketGuildUser)!;
    }

    public static string Id(this SocketGuildUser user)
    {
        return user!.Id.ToString();
    }
}
