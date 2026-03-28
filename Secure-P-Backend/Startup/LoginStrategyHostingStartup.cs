[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.LoginStrategyHostingStartup))]
namespace Secure_P_Backend.Startup;

public class LoginStrategyHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddLoginStrategies<string>();
        });
    }
}