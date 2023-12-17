using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Models.Hoyolab;
using System.Text;
using System.Text.Json;

namespace Discord.Bot.Services.Interactions;

public class Hoyolab(ILogger logger) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger = logger;

    [SlashCommand("checkin", "check connect to server spending")]
    public async Task CheckIn()
    {
        var user = Context.User.ToSocketGuild();
        _ = Task.Run(async () =>
        {
            using var client = new HttpClient();
            var checkIn = new CheckInRequest
            {
                DiscordId = user.Id(),
            };

            var payload = JsonSerializer.Serialize(checkIn);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("hoyolab.kaname-madoka.com/CheckIn", content);

            var responseJson = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("response {response}", responseJson);

            var result = JsonSerializer.Deserialize<CheckInResponse>(responseJson);

            var message = result?.Code == 0 ? "Check in success" : result?.Message;
            await Context.Channel.SendMessageAsync($"{user.Mention} {message}");
        });

        await RespondAsync("check in processing....");
    }
}
