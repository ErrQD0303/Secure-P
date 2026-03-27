[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.CacheServiceHostingStartup))]
namespace Secure_P_Backend.Startup;

public class CacheServiceHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            if (context.HostingEnvironment.IsEnvironment("Test"))
            {
                config.AddUserSecrets<ProgramMarker>();
            }
        });

        builder.ConfigureServices((context, services) =>
        {
            services.AddRedisCacheManager(context.Configuration);
        });
    }
}