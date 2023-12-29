namespace Dispatcher;
public interface ICommand<T>
{
    void Execute(T command);
}
