using Microsoft.Extensions.DependencyInjection;
using SecureP.Service.Abstraction;

namespace SecureP.Service.UploadService.Extensions;

public static class UploadServiceExtensions
{
    public static IServiceCollection AddUploadService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddTransient<IUploadService<TKey>, UploadService<TKey>>();

        return services;
    }
}