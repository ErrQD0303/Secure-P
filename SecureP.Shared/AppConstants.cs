namespace SecureP.Shared;

public static class AppConstants
{
    public static IdentityEntityFrameworkCoreConstants IdentityEntityFrameworkCore { get; } = new IdentityEntityFrameworkCoreConstants();
    public const string DefaultCacheInstanceName = "SecureP-Cache";
    public const string DefaultLoginProvider = "SecureP";
    public const string DefaultRoutePrefix = "api";
    public const bool EnableGDPR = true;
    public const string ImagesFolder = "images";
    public const string UserAvatarFolder = ImagesFolder + "/" + "user-avatar";
    public const string JwtConfiguresSection = "Jwt";
    public const string DefaultAdminEmail = "admin@SecureP.com";
    public const string DefaultAdminPassword = "Admin@123";
    public const string DefaultNormalUserEmail = "user@SecureP.com";
    public const string DefaultNormalUserPassword = "Admin@123";

    public class IdentityEntityFrameworkCoreConstants
    {
        public readonly string DefaultSchema = "Identity";
    }

    public class CacheOptions
    {
        public static long AbsoluteExpirationRelativeToNow { get; set; } = 60 * 60 * 24; // 4 hours
        public static long SlidingExpiration { get; set; } = 60 * 30; // 30 minutes

        public class RedisCacheOptions
        {
            public const string SectionName = "REDIS";
            public static string GetConfiguration(string serverUrl, string user, string password) => $"{serverUrl},user={user},password={password}";
        }
    }

    public class CachePermission
    {
        public const string RedisKeyName = DefaultCacheInstanceName + "-Permissions";
        public static string GetRedisKey(string userId) => $"{RedisKeyName}_{userId}";
    }

    public class SupportEmailType
    {
        public const string Default = "Normal";
        public const string OTP = "OTP";
        public const string ConfirmEmail = "ConfirmEmail";
        public const string ForgotPassword = "ForgotPassword";
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

        public class ForgotPasswordAppEmail
        {
            public const string Subject = "SecureP - Password Reset";
            public const string Message = "To reset your password, please click the link below: ";
            public const string HTTPMessage = "<div>To reset your password, please click the link below:</div><b>{0}</b>";
            public static string GetMessage(string resetLink) => string.Format(Message, resetLink);
            public static string GetHTTPMessage(string resetLink) => string.Format(HTTPMessage, resetLink);
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
            public const string UpdateProfile = "user/update-profile";
            public const string ChangePassword = "user/change-password";
            public const string ForgotPassword = "user/forgot-password";
            public const string ResetPassword = "user/reset-password";
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

        public class ParkingLocationController
        {
            public const string DefaultRoute = "parking-location";
        }

        public class ParkingRateController
        {
            public const string DefaultRoute = "parking-rate";
        }

        public class ParkingZoneController
        {
            public const string DefaultRoute = "parking-zone";
        }
    }
}