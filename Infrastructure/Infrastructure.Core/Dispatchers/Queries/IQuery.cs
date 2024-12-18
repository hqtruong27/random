namespace Infrastructure.Dispatchers;

public interface IQuery<out TResponse> : IRequest<TResponse>;