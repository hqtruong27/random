using System.Web;
using Microsoft.Playwright;

namespace Hoyoverse.Features.Hoyolab.Activities;

[Get]
[Route("hoyolab/redeem-code")]
public record RedeemCodeCommand(User User) : MediatR.IRequest;

public class GetFromGenshinImpactFandomCommandHandler(
    ILogger<GetFromGenshinImpactFandomCommandHandler> logger,
    HoyoverseDbContext context) : IRequestHandler<RedeemCodeCommand>
{
    public async Task Handle(RedeemCodeCommand request, CancellationToken cancellationToken)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            Proxy = null
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://genshin-impact.fandom.com/wiki/Promotional_Code",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        var tableLocator = page.Locator(".wikitable.sortable.tdl3.tdl4.jquery-tablesorter");

        var links = tableLocator.Locator("a[href^='https://genshin.hoyoverse.com/gift?code=']");

        List<string> promotionalCodes = [];
        int count = await links.CountAsync();
        if (count > 0)
        {
            logger.LogInformation("Matching link found!");

            for (int i = 0; i < count; i++)
            {
                var href = await links.Nth(i).GetAttributeAsync("href");
                if (href != null)
                {
                    var queryString = HttpUtility.ParseQueryString(new Uri(href).Query);
                    var code = queryString["code"];
                    if (code != null)
                    {
                        promotionalCodes.Add(code);
                    }
                }

                logger.LogInformation("Link {i}, {href}", i + 1, href);
            }
        }
        else
        {
            logger.LogWarning("No matching link found.");
        }

        await browser.CloseAsync();

        var setting = await context.Options
           .AsQueryable()
           .FirstOrDefaultAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        foreach (var code in promotionalCodes)
        {
            foreach (var account in request.User.Hoyolabs)
            {
                var giAccount = account.Games.Exists(game => game == HoyolabGame.GenshinImpact);
                if (giAccount)
                {
                    await GetAsync(account, code);
                }
            }
        }
    }

    private static async Task<CheckInResponse> GetAsync(HoyolabAccount hoyolab, string code)
    {
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
        var url = $"https://sg-hk4e-api.hoyoverse.com/common/apicdkey/api/webExchangeCdkey?uid=839631094&region=os_asia&lang=en&cdkey={code}&game_biz=hk4e_global&sLangKey=en-us";
        var response = await client.GetAsync(url);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

        await Task.Delay(5001);
        return result!;
    }
}
