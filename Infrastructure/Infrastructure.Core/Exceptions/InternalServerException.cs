namespace Infrastructure.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, object details) : base(message)
    {
        Details = details;
    }

    public object? Details { get; }
}
