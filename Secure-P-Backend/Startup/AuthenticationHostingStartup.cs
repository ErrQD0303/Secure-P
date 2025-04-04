[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.AuthenticationHostingStartup))]
namespace Secure_P_Backend.Startup;

public class AuthenticationHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}