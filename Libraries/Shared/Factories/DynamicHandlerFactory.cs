namespace Shared.Factories;

internal class DynamicHandler(ISender sender)
{
    public Task<TResponse> Handle<TInput, TResponse>(TInput input)
        where TInput : IRequest<TResponse>
        => sender.Send(input);

    public Task HandleNoResponse<TInput>(TInput input)
        where TInput : IRequest
        => sender.Send(input);
}

public class DynamicHandlerFactory
{
    public static Delegate CreateForGenericRequest(Type iRequestInterface, Type inputType, IServiceProvider serviceProvider)
    {
        var responseType = iRequestInterface.GetGenericArguments()[0];
        var delegateType = typeof(Func<,>)
            .MakeGenericType(inputType, typeof(Task<>)
                .MakeGenericType(responseType));

        var method = typeof(DynamicHandler)
            .GetMethod(nameof(DynamicHandler.Handle))?
            .MakeGenericMethod(inputType, responseType);

        var scope = serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return method == null
            ? throw new InvalidOperationException(
                $"Handler method not found for generic IRequest<>"
                )
            : Delegate.CreateDelegate(delegateType, new DynamicHandler(sender), method);
    }

    public static Delegate CreateForNonGenericRequest(Type inputType, IServiceProvider serviceProvider)
    {
        var delegateType = typeof(Func<,>).MakeGenericType(inputType, typeof(Task));
        var method = typeof(DynamicHandler)
            .GetMethod(nameof(DynamicHandler.HandleNoResponse))?
            .MakeGenericMethod(inputType);

        var scope = serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return method == null
            ? throw new InvalidOperationException(
                $"Handler method not found for non-generic IRequest"
                )
            : Delegate.CreateDelegate(delegateType, new DynamicHandler(sender), method);
    }
}
