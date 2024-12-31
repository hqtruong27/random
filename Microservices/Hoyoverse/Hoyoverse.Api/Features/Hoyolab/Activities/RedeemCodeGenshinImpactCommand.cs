using System.Web;
using Hoyoverse.Features.Hoyolab.Events;
using Microsoft.Playwright;

namespace Hoyoverse.Features.Hoyolab.Activities;

[Post("hoyolab/activities/redeem-code/genshin-impact")]
public record RedeemCodeGenshinImpactCommand(HoyolabAccount Account) : ICommand;

public class RedeemCodeGenshinImpactCommandHandler(ILogger<RedeemCodeGenshinImpactCommandHandler> logger) : CommandHandler<RedeemCodeGenshinImpactCommand>
{
    public override async Task Handle(RedeemCodeGenshinImpactCommand request, CancellationToken cancellationToken)
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

        await page.GotoAsync(config.GenshinImpact.UrlRedeem, new()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 1000 * 60
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

        var setting = await Context.Queries<Option>().FirstAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        List<RedeemCodeMessage> redeems = [];

        foreach (var code in promotionalCodes.Take(3))
        {
            var response = await GetAsync(request.Account, config.GenshinImpact.Url, code);
            redeems.Add(new RedeemCodeMessage
            {
                Code = code,
                Message = response.Message
            });

            await Task.Delay(5000 + 10, cancellationToken);
        }

        await Event.PublishAsync(new RedeemCodeRedeemed
        {
            Redeems = redeems,
            Discord = new()
            {
                GuildId = 735540677294948414,
                ChannelId = 735543117163397141,
                Game = "Genshin Impact"

            }
        }, cancellationToken);
    }

    private static async Task<HoyoverseResponse> GetAsync(HoyolabAccount hoyolab, string url, string code)
    {
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);

        var response = await client.GetAsync(string.Format(url, code));

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<HoyoverseResponse>(stream);

        return result!;
    }
}
