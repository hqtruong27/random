using Dispatcher.Wrappers;
using System.Collections.Concurrent;

namespace Dispatcher.Extensions;
public static class RequestHandlerExtensions
{
    public static THandler Get<THandler>(this ConcurrentDictionary<Type, RequestHandler> handlers, object request, Type responseType = default!)
    {
        var requestHandler = handlers.GetOrAdd(request.GetType(), requestType =>
        {
            var wrapperType = responseType != default
                 ? typeof(RequestHandlerWrapperImplement<,>).MakeGenericType(requestType, responseType)
                 : typeof(RequestHandlerWrapperImplement<>).MakeGenericType(requestType);

            var wrapper = Activator.CreateInstance(wrapperType)
                ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");

            return (RequestHandler)wrapper;
        });

        return requestHandler is THandler handler
            ? handler
            : throw new InvalidOperationException($"Cannot convert RequestHandler to {typeof(THandler)}");
    }
}
