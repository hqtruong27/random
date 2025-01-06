using Hoyoverse.Features.GenshinImpact.Commands;
using Hoyoverse.Features.StarRail.Commands;
using Infrastructure.Quartz;

namespace Hoyoverse.QuartzJobs;

public class RedeemCodeCommandJob : Job
{
    protected override async Task Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var users = await Context.Queries<User>().ToListAsync(cancellationToken);
        foreach (var account in users.SelectMany(x => x.Accounts("hoyolab")))
        {
            var hoyolabAccount = account.FromJson<HoyolabAccount>();
            if (hoyolabAccount == null) continue;
            foreach (var game in hoyolabAccount.Games)
            {
                switch (game)
                {
                    case LinkedAccountGame.GenshinImpact:
                        await Sender.Send(new RedeemCodeGenshinImpactCommand(account), cancellationToken);
                        break;
                    case LinkedAccountGame.StarRail:
                        await Sender.Send(new RedeemCodeStarRailCommand(account), cancellationToken);
                        break;
                    case LinkedAccountGame.Hi3:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
