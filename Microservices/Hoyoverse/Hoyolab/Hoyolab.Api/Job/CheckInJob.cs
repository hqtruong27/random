using Hoyolab.Api.Features.Activity;
using MongoDB.Bson;
using MongoDB.Driver;
using Quartz;

namespace Hoyolab.Api.Job;

public class CheckInJob(IDispatcher dispatcher, IRepository<User, ObjectId> repository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var users = await repository.FindAsync(x => x.Hoyolabs.Any(i => i.IsAutoCheckIn));

        //TODO: use parallel processor
        foreach (var user in await users.ToListAsync())
        {
            await dispatcher.Send(new AutoCheckInCommand
            {
                User = user
            });
        }
    }
}
