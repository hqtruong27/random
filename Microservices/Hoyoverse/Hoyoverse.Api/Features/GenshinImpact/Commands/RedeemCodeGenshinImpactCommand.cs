using System.Web;
using Hoyoverse.Features.Hoyolab.Events;
using Microsoft.Playwright;

namespace Hoyoverse.Features.GenshinImpact.Commands;

[Tags("GenshinImpact")]
[Post("genshin-impact/redeem-code/genshin-impact")]
public record RedeemCodeGenshinImpactCommand(LinkedAccount Account) : ICommand;

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

        var redemptionCodes = await Context
            .Queries<RedemptionCode>()
            .Where(x => x.Type == "GenshinImpact" && promotionalCodes.Contains(x.Code))
            .ToListAsync(cancellationToken);

        promotionalCodes = [.. promotionalCodes.Except(redemptionCodes.Select(x => x.Code))];
        if (promotionalCodes.Count == 0)
        {
            logger.LogWarning("No new promotional codes found.");
            return;
        }

        await Context.Set<RedemptionCode>().BulkInsertAsync(
            promotionalCodes.Select(code => new RedemptionCode
            {
                Code = code,
                Type = "GenshinImpact"
            }),
            cancellationToken
            );

        var setting = await Context.Queries<Option>().FirstAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        List<RedeemCodeMessage> redeems = [];

        foreach (var code in promotionalCodes)
        {
            var response = await GetAsync(request.Account, config.GenshinImpact.Url, code);
            redeems.Add(new RedeemCodeMessage
            {
                Code = code,
                Message = response.Message
            });

            await Task.Delay(1002 * 5, cancellationToken);
        }

        var hoyolabAccount = request.Account.FromJson<HoyolabAccount>()!;
        await Event.PublishAsync(new RedeemCodeRedeemed
        {
            Redeems = redeems,
            Discord = new()
            {
                GuildId = hoyolabAccount.GuildId,
                ChannelId = hoyolabAccount.ChannelId,
                Game = "Genshin Impact"

            }
        }, cancellationToken);
    }

    private static async Task<HoyoverseResponse> GetAsync(LinkedAccount account, string url, string code)
    {
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("Cookie", account.Token);

        var response = await client.GetAsync(string.Format(url, code));

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<HoyoverseResponse>(stream);

        return result!;
    }
}
