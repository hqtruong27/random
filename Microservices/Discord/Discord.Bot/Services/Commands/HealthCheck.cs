using Discord.Interactions;

namespace Discord.Bot.Services.Commands;

public class HealthCheck : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "check bot live!")]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!");
    }
}
