namespace main.Middleware;

public class GlobalExceptionMiddleware(
    RequestDelegate _next,
    ILogger<GlobalExceptionMiddleware> _logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
        context.Response.StatusCode = StatusCode;
        var response = new { error = new { message = exception.Message } };
        await context.Response.WriteAsJsonAsync(response);
    }
}
