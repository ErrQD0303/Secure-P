using SecureP.Authorization;

[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.AuthorizationHostingStartup))]
namespace Secure_P_Backend.Startup;

/// <summary>
/// A hosting startup class that configures authorization services for the Secure-P Backend application.
/// </summary>
public class AuthorizationHostingStartup : IHostingStartup
{
    /// <summary>
    /// Configures the authorization services for the application
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddAppPermissionAuthorization<string>();
        });
    }
}