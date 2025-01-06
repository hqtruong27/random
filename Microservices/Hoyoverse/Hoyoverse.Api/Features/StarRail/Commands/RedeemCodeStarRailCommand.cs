using System.Web;
using Hoyoverse.Features.Hoyolab.Events;
using Microsoft.Playwright;

namespace Hoyoverse.Features.StarRail.Commands;

public sealed record RedeemCodeStarRailCommand(LinkedAccount Account) : ICommand;

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
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 1000 * 60
        });

        var tableLocator = page.Locator(".wikitable.sortable.tdl3.tdl4.jquery-tablesorter");

        var links = tableLocator.Locator("a[href^='https://hsr.hoyoverse.com/gift?code=']");

        List<string> promotionalCodes = [];
        int count = await links.CountAsync();
        if (count > 0)
        {
            logger.LogInformation("Matching link found!");

            for (int i = 0; i < 10; i++)
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
           .Where(x => x.Type == "StarRail" && promotionalCodes.Contains(x.Code))
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
                Type = "StarRail"
            }),
            cancellationToken
            );

        var setting = await Context.Queries<Option>().FirstAsync(x => x.Key == "ACTIVITY_CONFIG", cancellationToken);
        var configure = BsonSerializer.Deserialize<ActivityConfig>(setting.Value);

        List<RedeemCodeMessage> redeems = [];

        foreach (var code in promotionalCodes)
        {
            var response = await PostAsync(request.Account, config.Hsr.Url, code);
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
                Game = "Star Rail"
            }
        }, cancellationToken);
    }

    private static async Task<HoyoverseResponse> PostAsync(LinkedAccount account, string url, string code)
    {
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("Cookie", account.Token);
        var body = new
        {
            lang = "en",
            game_biz = "hkrpg_global",
            uid = "830364485",
            region = "prod_official_asia",
            cdkey = code,
            platform = "4"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
            );

        var response = await client.PostAsync(url, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<HoyoverseResponse>(stream);

        return result!;
    }
}