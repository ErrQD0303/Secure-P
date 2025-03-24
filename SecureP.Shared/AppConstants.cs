namespace SecureP.Shared;

public static class AppConstants
{
    public static IdentityEntityFrameworkCoreConstants IdentityEntityFrameworkCore { get; } = new IdentityEntityFrameworkCoreConstants();
    public const string DefaultLoginProvider = "SecureP";
    public const string DefaultRoutePrefix = "api";

    public class IdentityEntityFrameworkCoreConstants
    {
        public readonly string DefaultSchema = "Identity";
    }
    public class SupportEmailType
    {
        public const string Default = "Normal";
        public const string OTP = "OTP";
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
}