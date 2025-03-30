using Microsoft.AspNetCore.Identity;

namespace SecureP.Shared;

public class AppIdentityErrors
{
    public static IdentityError UserNotFound => new()
    {
        Code = "UserNotFound",
        Description = "User not found"
    };

    public static IdentityError EmailNotConfirmed => new()
    {
        Code = "summary",
        Description = "Email not confirmed"
    };

    public static IdentityError InvalidPassword => new()
    {
        Code = "password",
        Description = "Invalid password"
    };

    public static IdentityError InvalidConfirmPassword => new()
    {
        Code = "confirm_password",
        Description = "Invalid confirm password"
    };

    public static IdentityError InvalidEmail => new()
    {
        Code = "email",
        Description = "Invalid email"
    };

    public static IdentityError InvalidToken => new()
    {
        Code = "token",
        Description = "Invalid token"
    };

    public static IdentityError InvalidRequest => new()
    {
        Code = "summary",
        Description = "Invalid request"
    };

    public static IdentityError UnknownError => new()
    {
        Code = "UnknownError",
        Description = "An unknown error occurred"
    };
}