namespace Infrastructure.Dispatchers;

public abstract class QueryHandler<TQuery, TResponse> : AmbientContextHandler, IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
}