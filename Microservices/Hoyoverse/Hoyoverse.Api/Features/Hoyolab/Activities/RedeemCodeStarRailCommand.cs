using System.Web;
using Microsoft.Playwright;

namespace Hoyoverse.Features.Hoyolab.Activities;

public sealed record RedeemCodeStarRailCommand(HoyolabAccount Account) : ICommand;

public class RedeemCodeStarRailCommandHandler(ILogger<RedeemCodeStarRailCommandHandler> logger) : CommandHandler<RedeemCodeStarRailCommand>
{
    public override async Task Handle(RedeemCodeStarRailCommand request, CancellationToken cancellationToken)
    {
        var options = await Context.Queries<Option>().FirstAsync(
            x => x.Key == "REDEEM_CODE_CONFIG",
            cancellationToken
            );

        var config = BsonSerializer.Deserialize<RedeemCodeConfig>(options.Value);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            Proxy = null
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync(config.Hsr.UrlRedeem, new()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });

        var tableLocator = page.Locator(".wikitable sortable tdl3 tdl4 jquery-tablesorter");

        var links = tableLocator.Locator("a[href^='https://hsr.hoyoverse.com/gift?code=']");

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

        var setting = await Context.Queries<Option>().FirstAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        foreach (var code in promotionalCodes)
        {
            await GetAsync(request.Account, config.Hsr.Url, code);
        }
    }

    private static async Task<CheckInResponse> GetAsync(HoyolabAccount hoyolab, string url, string code)
    {
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);

        var response = await client.GetAsync(string.Format(url, code));

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

        await Task.Delay(5001);
        return result!;
    }
}