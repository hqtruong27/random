namespace Infrastructure.Extensions;

public class AmbientContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        AmbientContext.Current = new() { ServiceProvider = serviceProvider };

        try
        {
            await next(context);
        }
        finally
        {
            AmbientContext.Current = new() { ServiceProvider = null! };
        }
    }
}
