using Microsoft.AspNetCore.Identity;

namespace SecureP.Service.Abstraction.Exceptions;

public class UserRegisterException : Exception
{
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