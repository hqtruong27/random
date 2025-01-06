namespace Hoyoverse.Features.Hoyolab.Commands;

public record AutoCheckInCommand(User User) : ICommand;

public class AutoCheckInCommandHandler(HoyoverseDbContext context) : CommandHandler<AutoCheckInCommand>
{
    public override async Task Handle(AutoCheckInCommand request, CancellationToken cancellationToken)
    {
        var setting = await context.Options
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        foreach (var account in request.User.Accounts("hoyolab"))
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
                        break;
                    case LinkedAccountGame.StarRail:
                        var hsr = await PostAsync(configure.Hsr, account);
                        hsr.Name = "HSR";
                        break;
                    case LinkedAccountGame.Hi3:
                        var hi3 = await PostAsync(configure.Hi3, account);
                        hi3.Name = "Hi3";
                        break;
                }
            }
        }
    }

    private static async Task<HoyoverseResponse> PostAsync(Config config, LinkedAccount account)
    {
        using HttpClient client = new();

        var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
        client.DefaultRequestHeaders.Add("Cookie", account.Token);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(config.CheckInUrl, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<HoyoverseResponse>(stream);

        return result!;
    }
}