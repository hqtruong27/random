namespace Discord.Bot.Features.Hoyoverse.Hoyolab;

public class CheckIn(HoyolabOptions settings) : BaseCommandModule
{
    [Command("checkin")]
    public async Task CheckInAsync(CommandContext ctx)
    {
        using var client = new HttpClient();
        var checkIn = new CheckInRequest
        {
            DiscordId = ctx.UserId.ToString(),
        };

        var payload = JsonSerializer.Serialize(checkIn);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{settings.Gateway}/activity/check-in", content);

        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<List<CheckInResponse>>(responseJson)!;

        foreach (var item in result)
        {
            var message = item.Code == 0 ? "Check in success" : item.Message;
            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} {item.Name}: {message}");
        }
    }
}