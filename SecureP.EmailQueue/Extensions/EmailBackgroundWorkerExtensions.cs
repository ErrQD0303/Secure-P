using Microsoft.Extensions.DependencyInjection;
using SecureP.Service.Abstraction;

namespace SecureP.EmailQueue.Extensions;

public static class EmailBackgroundWorkerExtensions
{
    public static IServiceCollection AddEmailBackgroundWorker(this IServiceCollection services)
    {
        services.AddSingleton<IEmailTaskQueue, EmailTaskQueue>();
        services.AddHostedService<EmailBackgroundWorker>();
        return services;
    }
}