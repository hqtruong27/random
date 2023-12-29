namespace Core;
public interface ICommand<T>
{
    void Execute(T command);
}
