using Hoyoverse.Features.Hoyolab.Activities;
using Infrastructure.Quartz;

namespace Hoyoverse.QuartzJobs;

public class CheckInJob : Job
{
    protected override async Task Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var users = await Context
            .Queries<User>()
            .Where(x => x.Hoyolabs.Any(i => i.IsAutoCheckIn))
            .ToListAsync(cancellationToken);

        //TODO: use parallel processor
        foreach (var user in users)
        {
            await Sender.Send(new AutoCheckInCommand(user), cancellationToken);
        }
    }
}
