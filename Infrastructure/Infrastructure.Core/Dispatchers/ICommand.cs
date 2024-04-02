namespace Infrastructure.Core.Dispatchers;

public interface ICommand : ICommand<Unit>;
public interface ICommand<out TResponse> : IRequest<TResponse>;