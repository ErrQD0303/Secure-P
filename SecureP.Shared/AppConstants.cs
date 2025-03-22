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
}