namespace Infrastructure.Dispatchers;

public abstract class CommandHandler<TCommand> : Handler, ICommandHandler<TCommand>
    where TCommand : ICommand<Unit>
{
    public abstract Task<Unit> Handle(TCommand request, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResponse> : Handler, ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
}