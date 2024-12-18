namespace Infrastructure.Exceptions;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError("[ERROR] Message: {Message}", exception.Message);

        var (message, title, statusCode) = exception switch
        {
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                StatusCodes.Status404NotFound
            ),
            BadRequestException or ValidationException =>
            (
                exception.Message,
                exception.GetType().Name,
                StatusCodes.Status400BadRequest
            ),
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                StatusCodes.Status500InternalServerError
            )
        };

        httpContext.Response.StatusCode = statusCode;
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = message,
            Status = statusCode,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

        if (exception is ValidationException validationException)
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
