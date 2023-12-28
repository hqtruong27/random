using Hoyolab.Services.Interfaces;
using Hoyoverse.Infrastructure.Entities;
using Hoyoverse.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Quartz;

namespace Hoyolab.Api.Job;

public class CheckInJob(IActivityService activityService, IRepository<User, ObjectId> repository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var users = await repository.FindAsync(x => x.Hoyolabs.Any(i => i.IsAutoCheckIn));

        //TODO: use parallel processor
        foreach (var user in await users.ToListAsync())
        {
            await activityService.AutoCheckInAsync(user);
        }
    }
}
