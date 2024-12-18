using Hoyoverse.Features.Hoyolab.Activities;

namespace Hoyoverse.Job;

public class CheckInJob(ISender sender, HoyoverseDbContext database) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var users = await database.Users.FindAsync(x => x.Hoyolabs.Any(i => i.IsAutoCheckIn));
        //TODO: use parallel processor
        foreach (var user in await users.ToListAsync())
        {
            await sender.Send(new AutoCheckInCommand(user));
        }
    }
}
