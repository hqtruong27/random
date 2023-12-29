using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Wrappers;

public abstract class RequestHandler
{
    public abstract Task<object?> Handle(object request, IServiceProvider service, CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper<TResponse> : RequestHandler
{
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider service, CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper : RequestHandler
{
    public abstract Task Handle(IRequest request, IServiceProvider service,
        CancellationToken cancellationToken);
}

public class RequestHandlerWrapperImplement<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest
{
    public override Task Handle(IRequest request, IServiceProvider service, CancellationToken cancellationToken)
    {
        return service.GetRequiredService<IRequestHandler<TRequest>>().Handle((TRequest)request, cancellationToken);
    }

    public override async Task<object?> Handle(object request, IServiceProvider service, CancellationToken cancellationToken)
    {
        return await Handle(request, service, cancellationToken).ConfigureAwait(false);
    }
}

public class RequestHandlerWrapperImplement<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> Handle(object request, IServiceProvider service, CancellationToken cancellationToken)
    {
        return await Handle((IRequest<TResponse>)request, service, cancellationToken).ConfigureAwait(false);
    }

    public override Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider service, CancellationToken cancellationToken)
    {
        return service.GetRequiredService<IRequestHandler<TRequest, TResponse>>().Handle((TRequest)request, cancellationToken);
    }
}