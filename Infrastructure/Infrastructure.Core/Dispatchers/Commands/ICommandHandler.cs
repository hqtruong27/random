namespace Infrastructure.Dispatchers;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>  where TCommand : ICommand, IRequest;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull;