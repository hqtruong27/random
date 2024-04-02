namespace Infrastructure.Core.Dispatchers;

public interface IQuery<out TResponse> : IRequest<TResponse>;