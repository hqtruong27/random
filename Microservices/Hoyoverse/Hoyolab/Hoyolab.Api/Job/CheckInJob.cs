using Hoyolab.Api.Features.Activities;
using Quartz;

namespace Hoyolab.Api.Job;

public class CheckInJob(IDispatcher dispatcher, IDatabaseContext database) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var users = await database.Users.FindAsync(x => x.Hoyolabs.Any(i => i.IsAutoCheckIn));
        //TODO: use parallel processor
        foreach (var user in await users.ToListAsync())
        {
            await dispatcher.Send(new AutoCheckInCommand(user));
        }
    }
}
