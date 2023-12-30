using Dispatcher.Wrappers;
using System.Collections.Concurrent;

namespace Dispatcher.Extensions;
public static class RequestHandlerExtensions
{
    public static THandler Get<THandler>(this ConcurrentDictionary<Type, RequestHandler> handlers, object request, Type? responseType = default)
    {
        var requestHandler = handlers.GetOrAdd(request.GetType(), requestType =>
        {
            var wrapperType = responseType == default
                    ? typeof(RequestHandlerWrapperImplement<>).MakeGenericType(requestType)
                    : typeof(RequestHandlerWrapperImplement<,>).MakeGenericType(requestType, responseType);

            var wrapper = Activator.CreateInstance(wrapperType);

            return wrapper != null
                ? (RequestHandler)wrapper
                : throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
        });

        return ConvertTo<THandler>(requestHandler);
    }

    private static T ConvertTo<T>(object obj)
    {
        return obj is T converted
            ? converted
            : throw new InvalidCastException($"Cannot convert object of type {obj.GetType()} to type {typeof(T)}.");
    }
}
