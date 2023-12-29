namespace Dispatcher;

public interface ICommandHandler<TCommand>
{
    void Handle(TCommand command);
}