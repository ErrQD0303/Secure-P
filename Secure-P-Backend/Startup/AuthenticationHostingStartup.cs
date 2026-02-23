[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.AuthenticationHostingStartup))]
namespace Secure_P_Backend.Startup;

/// <summary>
/// A hosting startup class that configures authentication services for the Secure-P Backend application.
/// </summary>
public class AuthenticationHostingStartup : IHostingStartup
{
    /// <summary>
    /// Configures the authentication services for the application. This method is called during application startup to set up the necessary authentication schemes and options. In this implementation, it currently does not add any services, but it can be extended in the future to include additional authentication configurations as needed. The method receives an IWebHostBuilder instance, allowing for configuration of services and middleware related to authentication within the application's hosting environment.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}