using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Cache.Abstraction;
using SecureP.Cache.Abstraction.Options;
using SecureP.Cache.RedisCacheManager.Options;
using SecureP.Shared;
using StackExchange.Redis;

namespace SecureP.Cache.RedisCacheManager.Extensions;

public static class RedisCacheManagerExtensions
{
    public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(options =>
        {
            options.AbsoluteExpirationRelativeToNow = AppConstants.CacheOptions.AbsoluteExpirationRelativeToNow;
            options.SlidingExpiration = AppConstants.CacheOptions.SlidingExpiration;
        });

        services.Configure<RedisOptions>(configuration.GetSection(AppConstants.CacheOptions.RedisCacheOptions.SectionName));

        var redisOptions = configuration.GetSection(AppConstants.CacheOptions.RedisCacheOptions.SectionName).Get<RedisOptions>() ?? throw new ArgumentNullException(nameof(RedisOptions), "Redis options cannot be null.");

        var configurationString = AppConstants.CacheOptions.RedisCacheOptions.GetConfiguration(redisOptions.SERVER_URL, redisOptions.USER, redisOptions.PASSWORD);

        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configurationString));
        services.AddSingleton<ICacheManager, RedisCacheManager>();

        return services;
    }
}