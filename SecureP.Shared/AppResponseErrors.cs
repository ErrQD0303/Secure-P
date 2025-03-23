namespace SecureP.Shared;

public class AppResponseErrors
{
    public class UserRegisterErrors
    {
        public static Dictionary<string, string> UserRegistrationFailed => new()
        {
            { "summary", "User registration failed" }
        };
    }

    public class UserLoginErrors
    {
        public static Dictionary<string, string> UserLoginFailed => new()
        {
            { "summary", "User registration failed" }
        };
    }
}