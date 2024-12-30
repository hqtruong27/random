namespace Infrastructure.Dispatchers.Handlers;

public class QuartzJobContextHandler : AmbientContextHandler
{
    protected static ISender Sender => AmbientContext.Current.ServiceProvider.GetRequiredService<ISender>();
}
