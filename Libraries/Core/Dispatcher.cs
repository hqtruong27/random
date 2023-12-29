using System.Collections.Concurrent;
using Core.Extensions;
using Core.Wrappers;

namespace Core;

public interface IDispatcher : ISender { }

public class Dispatcher(IServiceProvider service) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, RequestHandler> _requestHandlers = [];

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handler = _requestHandlers.Get<RequestHandlerWrapper<TResponse>>(request, typeof(TResponse));

        return handler.Handle(request, service, cancellationToken);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
     where TRequest : IRequest
    {
        var handler = _requestHandlers.Get<RequestHandlerWrapper>(request);

        return handler.Handle(request, service, cancellationToken);
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    //public TResult Send<TResult>(IRequest<TResult> request)
    //{
    //    var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResult));
    //    dynamic handler = serviceProvider.GetRequiredService(handlerType);

    //    return handler == null
    //        ? throw new InvalidOperationException($"No request handler registered for {request.GetType().Name}")
    //        : (TResult)handler.Handle((dynamic)request);
    //}
}