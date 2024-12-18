namespace Infrastructure.Exceptions;

public class InternalServerException : Exception
{
    public InternalServerException(string message) : base(message)
    {
    }

    public InternalServerException(string message, object details) : base(message)
    {
        Details = details;
    }

    public object? Details { get; }
}
