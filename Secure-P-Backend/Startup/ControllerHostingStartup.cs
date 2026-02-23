[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.ControllerHostingStartup))]
namespace Secure_P_Backend.Startup;

/// <summary>
/// A hosting startup class that configures controller services for the Secure-P Backend application.
/// </summary>
public class ControllerHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddControllers(o =>
            {
                o.UseRoutePrefix(AppConstants.DefaultRoutePrefix);
            });
        });
    }
}