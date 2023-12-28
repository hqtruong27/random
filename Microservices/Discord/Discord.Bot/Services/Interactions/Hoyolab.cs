using Discord.Bot.Models;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Models.Hoyolab;
using System.Text;
using System.Text.Json;

namespace Discord.Bot.Services.Interactions;

public class Hoyolab(ILogger<Hoyolab> logger, HoyolabSettings settings) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly HoyolabSettings _settings = settings;
    private readonly ILogger _logger = logger;

    [SlashCommand("check-in", "daily check in")]
    public async Task CheckIn()
    {
        var user = Context.User.ToSocketGuild();
        _ = Task.Run(async () =>
        {
            _logger.LogInformation("Start check in");
            using var client = new HttpClient();
            var checkIn = new CheckInRequest
            {
                DiscordId = user.Id(),
            };

            var payload = JsonSerializer.Serialize(checkIn);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_settings.Gateway}/activity/check-in", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("response {response}", responseJson);

            var result = JsonSerializer.Deserialize<List<CheckInResponse>>(responseJson)!;

            foreach (var item in result)
            {
                var message = item.Code == 0 ? "Check in success" : item.Message;
                await Context.Channel.SendMessageAsync($"{user.Mention} {message}");
            }
        });

        await RespondAsync("check in ....");
    }
}
