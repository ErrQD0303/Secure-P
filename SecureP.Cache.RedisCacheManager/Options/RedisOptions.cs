namespace SecureP.Cache.RedisCacheManager.Options;

public class RedisOptions
{
    public string SERVER_URL { get; set; } = string.Empty;
    public string INSTANCE_NAME { get; set; } = string.Empty;
    public string USER { get; set; } = "default";
    public string PASSWORD { get; set; } = string.Empty;
}