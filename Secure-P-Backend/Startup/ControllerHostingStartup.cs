[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.ControllerHostingStartup))]
namespace Secure_P_Backend.Startup;

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