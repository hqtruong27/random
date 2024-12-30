using Hoyoverse.Features.Hoyolab.Activities;
using Infrastructure.Quartz;

namespace Hoyoverse.QuartzJobs;

public class RedeemCodeCommandJob : Job
{
    protected override async Task Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var accounts = await Context.Queries<User>().ToListAsync(cancellationToken);
        foreach (var account in accounts)
        {
            var giAccount = account.Hoyolabs.FirstOrDefault(account => account.Games.Contains(HoyolabGame.GenshinImpact));
            var hsrAccount = account.Hoyolabs.FirstOrDefault(account => account.Games.Contains(HoyolabGame.StarRail));
            if (giAccount != null)
            {
                await Sender.Send(new RedeemCodeGenshinImpactCommand(giAccount), cancellationToken);
            }
            if (hsrAccount != null)
            {
                await Sender.Send(new RedeemCodeStarRailCommand(hsrAccount), cancellationToken);
            }
        }
    }
}
