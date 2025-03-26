namespace SecureP.Shared;

public class AppResponses
{
    public class UserLogoutResponses
    {
        public const string UserLoggedOut = "User logged out";
        public const string UserLogoutFailed = "User failed to logout";

        public const string UserNotFound = "User not found";
    }

    public class UserRegisterResponses
    {
        public const string UserRegistered = "User registered";
        public const string UserRegistrationFailed = "User failed to register";
    }
    public class GetUserInfoResponses
    {
        public const string UserNotFound = "User not found";
        public const string UserFound = "User found";
    }
    public class UserLoginResponses
    {
        public const string UserLoggedIn = "Login successful";
        public const string UserLoggedInWaitForOTP = "Login successful, waiting for OTP";
        public const string UserLoginFailed = "User logged in failed, Invalid email or password!";
    }
    public class EmailConfirmationResponses
    {
        public const string EmailConfirmed = "Email confirmed";
        public const string EmailNotConfirmed = "Email not confirmed";
        public const string EmailConfirmationFailed = "Email confirmation failed";
        public const string UserNotFound = "User has been deleted";
    }
    public class ResendEmailConfirmationResponses
    {
        public const string UserNotFound = "User has been deleted";
        public const string EmailSent = "Email sent";
        public const string EmailNotSent = "Email not sent";
        public const string EmailIsNotMatch = "Email is not match";
    }
}