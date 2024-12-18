namespace Hoyoverse.Features.Hoyolab.Activities;

[Post]
[Route("activity/check-in")]
public record CheckInCommand(string DiscordId, DateTime Date) : IRequest<List<CheckInResponse>>;

public class CheckInCommandHandler(
    HoyoverseDbContext context,
    ILogger<CheckInCommandHandler> logger) : IRequestHandler<CheckInCommand, List<CheckInResponse>>
{
    public async Task<List<CheckInResponse>> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var option = await context.Options
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(option.Value);

        logger.LogInformation("config: {configure}", configure);

        var user = await context.Users
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Discord.Id == request.DiscordId, cancellationToken);

        if (user == null)
        {
            return
            [
                new CheckInResponse
                    {
                        Code = -1,
                        Message = "Login Discord first"
                    }
            ];
        }

        List<CheckInResponse> result = [];
        foreach (var hoyolab in user.Hoyolabs)
        {
            foreach (var account in hoyolab.Games)
            {
                switch (account)
                {
                    case HoyolabGame.GenshinImpact:
                        var gi = await PostAsync(configure.Genshin, hoyolab);
                        gi.Name = "GI";
                        result.Add(gi);
                        break;
                    case HoyolabGame.StarRail:
                        var hsr = await PostAsync(configure.Hsr, hoyolab);
                        hsr.Name = "HSR";
                        result.Add(hsr);
                        break;
                    case HoyolabGame.HonkaiImpact3:
                        var hi3 = await PostAsync(configure.Hi3, hoyolab);
                        hi3.Name = "Hi3";
                        result.Add(hi3);
                        break;
                    case HoyolabGame.ZenlessZoneZero:
                        break;
                }
            }
        }

        return result;
    }

    private async Task<CheckInResponse> PostAsync(Config config, HoyolabAccount hoyolab)
    {
        using HttpClient client = new();

        var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        logger.LogInformation("payload: {url}, {payload}", config.CheckInUrl, payload);
        var response = await client.PostAsync(config.CheckInUrl, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

        return result!;
    }
}