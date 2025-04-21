using SecureP.Authorization;

[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.AuthorizationHostingStartup))]
namespace Secure_P_Backend.Startup;

public class AuthorizationHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddAppPermissionAuthorization<string>();
        });
    }
}