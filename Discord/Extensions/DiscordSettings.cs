using Grpc.Net.Client;

namespace Models.Settings;

public class DiscordSettings
{
    public ulong GuildId { get; set; }
    public long ClientId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public GrpcChannel Channel { get; set; } = default!;
}
