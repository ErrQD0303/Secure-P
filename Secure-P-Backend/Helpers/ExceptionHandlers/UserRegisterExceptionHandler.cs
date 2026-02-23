
namespace SecureP.Helpers.ExceptionHandlers;

/// <summary>
/// Exception handler for user registration failures
/// </summary>
public class UserRegisterExceptionHandler : IExceptionHandler
{
    private ILogger<UserRegisterExceptionHandler> Logger { get; }

    public UserRegisterExceptionHandler(ILogger<UserRegisterExceptionHandler> logger)
    {
        Logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Only handle UserRegisterException, let other exceptions propagate to the global exception handler
        if (exception is not UserRegisterException ex)
        {
            Logger.LogError(exception, "An unexpected error occurred during user registration.");
            return false;
        }

        Logger.LogWarning(exception, "User registration validation failed.");

        // 2. Create standardized problem details response based on the IdentityErrors in the exception
        var errors = ex.IdentityErrors.ToDictionary(e => e.Code, e => new[] { e.Description });

        ValidationProblemDetails problemDetails = new(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = AppResponses.UserRegisterResponses.UserRegistrationFailed,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
        };

        // 34. Write the response and return the response to the client
        Logger.LogInformation("Returning validation errors for user registration failure: {@Errors}", errors);
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indicate that the exception has been handled    
    }
}