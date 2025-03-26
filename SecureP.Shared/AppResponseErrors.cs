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

    public class EmailConfirmationErrors
    {
        public static Dictionary<string, string> EmailConfirmationFailed => new()
        {
            { "summary", "Email confirmation failed" }
        };
    }
    public class ResendEmailConfirmationErrors
    {
        public static Dictionary<string, string> EmailIsNotMatch => new()
        {
            { "summary", "Email confirmation failed" },
            {"email", "Email is not matched with the email in the database"}
        };
    }
}