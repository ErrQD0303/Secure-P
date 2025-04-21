using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SecureP.Cache.Abstraction;
using SecureP.Cache.Abstraction.Options;
using StackExchange.Redis;

namespace SecureP.Cache.RedisCacheManager;

public class RedisCacheManager : ICacheManager
{
    public ILogger<RedisCacheManager> Logger { get; private set; }
    public StackExchange.Redis.IDatabase RedisCache { get; private set; }
    public CacheOptions CacheOptions { get; private set; }
    public DistributedCacheEntryOptions DistributedCacheEntryOptions { get; private set; }

    public RedisCacheManager(ILogger<RedisCacheManager> logger, IConnectionMultiplexer redis, IOptions<CacheOptions> cacheOptions)
    {
        Logger = logger;
        RedisCache = redis.GetDatabase();
        CacheOptions = cacheOptions.Value;
        DistributedCacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheOptions.AbsoluteExpirationRelativeToNow),
            SlidingExpiration = TimeSpan.FromSeconds(CacheOptions.SlidingExpiration)
        };
    }

    public async Task ClearAsync()
    {
        Logger.LogInformation("Clearing Redis Cache");
        var server = RedisCache.Multiplexer.GetServer(RedisCache.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(RedisCache.Database, "*").ToArray();
        var deleteTasks = keys.Select(key => RedisCache.KeyDeleteAsync(key)).ToArray();

        await Task.WhenAll(deleteTasks);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        Logger.LogInformation($"Checking if key {key} exists in Redis Cache");
        var value = await RedisCache.StringGetAsync(key);
        return !string.IsNullOrEmpty(value);
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        Logger.LogInformation($"Getting value for key {key} from Redis Cache");
        var byteValue = await RedisCache.StringGetAsync(key);
        if (string.IsNullOrEmpty(byteValue))
        {
            return default;
        }
        try
        {
            var value = Encoding.UTF8.GetString(byteValue!);

            return JsonConvert.DeserializeObject<T>(value!);
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, $"Error deserializing value for key {key} from Redis Cache: {ex.Message}");
            return default;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Unexpected error while getting value for key {key} from Redis Cache: {ex.Message}");
            return default;
        }
    }

    public Task RemoveAsync(string key)
    {
        Logger.LogInformation($"Removing key {key} from Redis Cache");
        return RedisCache.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null) where T : class
    {
        Logger.LogInformation($"Setting value for key {key} in Redis Cache");
        var byteValue = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        }));
        if (expirationTime.HasValue)
        {
            await RedisCache.StringSetAsync(key, byteValue, expirationTime.Value);
            return;
        }

        await RedisCache.StringSetAsync(key, byteValue, DistributedCacheEntryOptions.AbsoluteExpirationRelativeToNow);
    }
}
