namespace Hoyoverse.Persistence.Schemas;

[BsonIgnoreExtraElements]
public class User : AuditableEntity
{
    public string Username { get; set; } = default!;
    public ICollection<LinkedAccount> LinkedAccounts { get; set; } = default!;
    public LinkedAccount? DiscordAccount => GetAccount("discord");

    private LinkedAccount? GetAccount(string provider)
    {
        return Accounts(provider).Count == 0 ? null : Accounts(provider)[0];
    }

    public List<LinkedAccount> Accounts(string provider)
    {
        return [.. LinkedAccounts.Where(x => x.Provider == provider)];
    }
}

public record LinkedAccount
{
    public string Id { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Provider { get; set; } = default!;
    public string Token { get; set; } = default!;
    public bool IsAutoCheckIn { get; set; }
    public DateTime Created { get; set; }
    public string? Metadata { get; set; }
    public List<LinkedAccountGame> Games { get; set; } = [];

    public T? FromJson<T>() where T : class
    {
        return Metadata == null ? default : JsonSerializer.Deserialize<T>(Metadata);
    }
}

public record HoyolabAccount
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public List<LinkedAccountGame> Games { get; set; } = [];
}

public enum LinkedAccountGame
{
    GenshinImpact,
    StarRail,
    Hi3
}