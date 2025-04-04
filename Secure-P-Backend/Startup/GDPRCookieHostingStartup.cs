[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.GDPRCookieHostingStartup))]
namespace Secure_P_Backend.Startup;

public class GDPRCookieHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookieValue = "true";
                options.CheckConsentNeeded = context => AppConstants.EnableGDPR;
                options.ConsentCookie = new CookieBuilder
                {
                    Name = AppConstants.DefaultLoginProvider + "_ConsentCookie",
                    Expiration = TimeSpan.FromDays(90),
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    SecurePolicy = CookieSecurePolicy.Always,
                    HttpOnly = false
                };
            });
        });
    }
}