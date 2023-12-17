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
        var users = await (await repository.FindAsync(x => x.Hoyolab.IsAutoCheckIn)).ToListAsync();

        //TODO: use parallel processor
        foreach (var user in users)
        {
            await activityService.CheckInAsync(new() { DiscordId = user.Discord.Id });
        }
    }
}
