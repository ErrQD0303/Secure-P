[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.TestHostingStartup))]
namespace Secure_P_Backend.Startup;

public class TestHostingStartup : IHostingStartup
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
    }
}