using Microsoft.AspNetCore.Identity;

namespace SecureP.Service.Abstraction.Exceptions;

/// <summary>
/// User Registration Exception
/// </summary>
public class UserRegisterException : Exception
{
    /// <summary>
    /// A collection of IdentityError objects that represent the validation errors that occurred during user registration.
    /// </summary>
    public IEnumerable<IdentityError> IdentityErrors { get; set; }

    public UserRegisterException(string message, params IEnumerable<IdentityError> errors) : base(message)
    {
        IdentityErrors = errors;
    }

    public UserRegisterException(string message, Exception innerException, params IEnumerable<IdentityError> errors) : base(message, innerException)
    {
        IdentityErrors = errors;
    }
}