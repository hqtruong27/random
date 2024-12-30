using Quartz;
using Quartz.Spi;

namespace Infrastructure.Quartz;

public class QuartzJobContextListener(IServiceProvider serviceProvider) : IJobListener
{
    public string Name => "QuartzJobContextListener";

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var scope = serviceProvider.CreateScope();

        context.JobDetail.JobDataMap.Put(
            nameof(AmbientContext),
            new AmbientContext
            {
                ServiceProvider = scope.ServiceProvider
            });

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        var scope = context.JobDetail.JobDataMap.Get(nameof(AmbientContext)) as AmbientContext;

        scope = new() { ServiceProvider = null! };

        return Task.CompletedTask;
    }
}