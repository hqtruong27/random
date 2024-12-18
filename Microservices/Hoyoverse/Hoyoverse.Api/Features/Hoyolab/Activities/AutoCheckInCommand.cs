namespace Hoyoverse.Features.Hoyolab.Activities;

public record AutoCheckInCommand(User User) : IRequest;

public class AutoCheckInCommandHandler(HoyoverseDbContext context) : IRequestHandler<AutoCheckInCommand>
{
    public async Task Handle(AutoCheckInCommand request, CancellationToken cancellationToken)
    {
        var setting = await context.Options
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        foreach (var hoyolab in request.User.Hoyolabs)
        {
            foreach (var account in hoyolab.Games)
            {
                switch (account)
                {
                    case HoyolabGame.GenshinImpact:
                        var gi = await PostAsync(configure.Genshin, hoyolab);
                        gi.Name = "GI";
                        break;
                    case HoyolabGame.StarRail:
                        var hsr = await PostAsync(configure.Hsr, hoyolab);
                        hsr.Name = "HSR";
                        break;
                    case HoyolabGame.HonkaiImpact3:
                        var hi3 = await PostAsync(configure.Hi3, hoyolab);
                        hi3.Name = "Hi3";
                        break;
                    case HoyolabGame.ZenlessZoneZero:
                        break;
                }
            }
        }
    }

    private static async Task<CheckInResponse> PostAsync(Config config, HoyolabAccount hoyolab)
    {
        using HttpClient client = new();

        var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(config.CheckInUrl, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

        return result!;
    }
}