namespace SecureP.Cache.Abstraction.Options;

public class CacheOptions
{
    public long AbsoluteExpirationRelativeToNow { get; set; } = 60 * 60 * 24; // 4 hours
    public long SlidingExpiration { get; set; } = 60 * 30; // 30 minutes
}