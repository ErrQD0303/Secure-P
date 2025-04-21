namespace SecureP.Cache.Abstraction;

public interface ICacheManager
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null) where T : class;
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task ClearAsync();
}
