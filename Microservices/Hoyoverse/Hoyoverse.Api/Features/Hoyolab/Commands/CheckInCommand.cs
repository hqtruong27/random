namespace Hoyoverse.Features.Hoyolab.Commands;

[Post]
[Route("activity/check-in")]
public record CheckInCommand(string DiscordId, DateTime Date) : ICommand<List<HoyoverseResponse>>;

public class CheckInCommandHandler(
    HoyoverseDbContext context,
    ILogger<CheckInCommandHandler> logger) : CommandHandler<CheckInCommand, List<HoyoverseResponse>>
{
    public override async Task<List<HoyoverseResponse>> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var option = await context.Options
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(option.Value);

        logger.LogInformation("config: {configure}", configure);

        var user = await context.Users
            .AsQueryable()
            .FirstOrDefaultAsync(
                x => x.DiscordAccount != null && x.DiscordAccount.Id == request.DiscordId,
                cancellationToken
                );

        if (user == null)
        {
            return
            [
                new HoyoverseResponse
                    {
                        Code = -1,
                        Message = "Login Discord first"
                    }
            ];
        }

        List<HoyoverseResponse> result = [];
        foreach (var account in user.Accounts("hoyolab"))
        {
            var hoyolabAccount = account.FromJson<HoyolabAccount>();
            if (hoyolabAccount == null) continue;
            foreach (var game in hoyolabAccount.Games)
            {
                switch (game)
                {
                    case LinkedAccountGame.GenshinImpact:
                        var gi = await PostAsync(configure.Genshin, account);
                        gi.Name = "GI";
                        result.Add(gi);
                        break;
                    case LinkedAccountGame.StarRail:
                        var hsr = await PostAsync(configure.Hsr, account);
                        hsr.Name = "HSR";
                        result.Add(hsr);
                        break;
                    case LinkedAccountGame.Hi3:
                        var hi3 = await PostAsync(configure.Hi3, account);
                        hi3.Name = "Hi3";
                        result.Add(hi3);
                        break;
                }
            }
        }

        return result;
    }

    private async Task<HoyoverseResponse> PostAsync(Config config, LinkedAccount account)
    {
        using HttpClient client = new();

        var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
        client.DefaultRequestHeaders.Add("Cookie", account.Token);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        logger.LogInformation("payload: {url}, {payload}", config.CheckInUrl, payload);
        var response = await client.PostAsync(config.CheckInUrl, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<HoyoverseResponse>(stream);

        return result!;
    }
}