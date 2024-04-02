namespace Infrastructure.Core.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, object details) : base(message)
    {
        Details = details;
    }

    public object? Details { get; }
}
