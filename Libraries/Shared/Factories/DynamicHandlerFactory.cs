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
    public static Delegate CreateForGenericRequest(Type iRequestInterface, Type inputType, ISender sender)
    {
        var responseType = iRequestInterface.GetGenericArguments()[0];
        var delegateType = typeof(Func<,>)
            .MakeGenericType(inputType, typeof(Task<>)
                .MakeGenericType(responseType));
        var method = typeof(DynamicHandler)
            .GetMethod(nameof(DynamicHandler.Handle))?
            .MakeGenericMethod(inputType, responseType);

        return method == null
            ? throw new InvalidOperationException(
                $"Handler method not found for generic IRequest<>"
                )
            : Delegate.CreateDelegate(delegateType, new DynamicHandler(sender), method);
    }

    public static Delegate CreateForNonGenericRequest(Type inputType, ISender sender)
    {
        var delegateType = typeof(Func<,>).MakeGenericType(inputType, typeof(Task));
        var method = typeof(DynamicHandler)
            .GetMethod(nameof(DynamicHandler.HandleNoResponse))?
            .MakeGenericMethod(inputType);

        return method == null
            ? throw new InvalidOperationException(
                $"Handler method not found for non-generic IRequest"
                )
            : Delegate.CreateDelegate(delegateType, new DynamicHandler(sender), method);
    }
}
