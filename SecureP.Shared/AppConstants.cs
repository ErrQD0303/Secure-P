namespace SecureP.Shared;

public static class AppConstants
{
    public static IdentityEntityFrameworkCoreConstants IdentityEntityFrameworkCore { get; } = new IdentityEntityFrameworkCoreConstants();
    public const string DefaultLoginProvider = "SecureP";
    public const string DefaultRoutePrefix = "api";
    public const bool EnableGDPR = true;
    public const string ImagesFolder = "images";
    public const string UserAvatarFolder = ImagesFolder + "/" + "user-avatar";

    public class IdentityEntityFrameworkCoreConstants
    {
        public readonly string DefaultSchema = "Identity";
    }
    public class SupportEmailType
    {
        public const string Default = "Normal";
        public const string OTP = "OTP";
        public const string ConfirmEmail = "ConfirmEmail";
    }

    public class AppEmail
    {
        public class OTPAppEmail
        {
            public const string Subject = "SecureP - OTP Verification";
            public const string Message = "Your OTP code is: ";
            public const string HTTPMessage = "Your OTP code is: <b>{0}</b>";
            public static string GetMessage(string otp) => string.Format(Message, otp);
            public static string GetHTTPMessage(string otp) => string.Format(HTTPMessage, otp);
        }

        public class ConfirmAppEmail
        {
            public const string Subject = "SecureP - Email Confirmation";
            public const string Message = "Please confirm your email by clicking the link below: ";
            public const string HTTPMessage = "<div>Please confirm your email by clicking the link below:</div><b>{0}</b>";
            public static string GetMessage(string confirmLink) => string.Format(Message, confirmLink);
            public static string GetHTTPMessage(string confirmLink) => string.Format(HTTPMessage, confirmLink);
        }
    }

    public class OTPConstant
    {
        public static int ExpiryMinute => 10;
        public static string TemporaryCookieName => "SecureP-OTP-Email";
    }

    public class CookieNames
    {
        public const string RefreshToken = "refresh_token";
        public const string AccessToken = "access_token";
    }

    public class AppController
    {
        public class IdentityController
        {
            public const string DefaultRoute = "Identity";
            public const string Register = "user/register";
            public const string Login = "user/login";
            public const string OTPLogin = "user/otp-login";
            public const string Logout = "user/logout";
            public const string GetUserInfo = "user/getInfo";
            public const string ConfirmEmail = "user/confirm-email";
            public const string ResendEmailConfirmation = "user/resend-email-confirmation";
            public class AdminUser
            {
                public const string GetUser = "get-user/{id}";
                public const string GetAllUser = "get-users";
            }
        }

        public class TokenController
        {
            public const string DefaultRoute = "Token";
            public const string GenerateToken = "token";
            public const string RefreshToken = "refresh";
        }

        public class GDPRController
        {
            public const string DefaultRoute = "GDPR";
        }

        public class UploadController
        {
            public const string DefaultRoute = "Upload";
            public const string UploadAvatar = "avatar";
        }
    }
}