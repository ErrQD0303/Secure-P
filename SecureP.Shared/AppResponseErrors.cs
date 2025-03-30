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
            { "summary", "User Login failed" }
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

    public class UpdatePasswordErrors
    {
        public static Dictionary<string, object> UpdatePasswordFailed => new()
        {
            { "summary", "Update password failed" }
        };
        public static Dictionary<string, object> NewPasswordIsSameAsOldPassword => new()
        {
            { "summary", "Update password failed" },
            { "NewPassword", new Dictionary<string, string> {
                { "NewPassword", "New password is same as old password" }
            } }
        };
    }
}