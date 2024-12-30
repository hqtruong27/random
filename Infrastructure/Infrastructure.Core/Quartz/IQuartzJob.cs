using Infrastructure.Dispatchers.Handlers;
using Quartz;

namespace Infrastructure.Quartz;

public abstract class Job : QuartzJobContextHandler, IJob
{
    public virtual async Task Execute(IJobExecutionContext context)
    {
        var instance = context.JobDetail.JobDataMap.Get(nameof(AmbientContext))!;
        if (instance is AmbientContext ambientContext)
            AmbientContext.Current = ambientContext;

        await Execute(context, context.CancellationToken);
    }

    protected abstract Task Execute(IJobExecutionContext context, CancellationToken cancellationToken);
}