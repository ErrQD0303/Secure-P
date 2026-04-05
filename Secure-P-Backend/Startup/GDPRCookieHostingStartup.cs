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
                options.MinimumSameSitePolicy = SameSiteMode.None; // In Production, consider setting this to SameSiteMode.Strict or SameSiteMode.Lax based on your requirements
                options.ConsentCookieValue = "true";
                options.CheckConsentNeeded = context => AppConstants.EnableGDPR; // If it's true, the application ban all cookies by default and only allow cookies after user consent. If it's false, the application will allow cookies without user consent.
                options.ConsentCookie = new CookieBuilder
                {
                    Name = AppConstants.DefaultLoginProvider + "_ConsentCookie",
                    Expiration = TimeSpan.FromDays(90),
                    IsEssential = true, // means that the cookie is essential for the application to function and should be allowed even if the user has not given consent for non-essential cookies.
                    SameSite = SameSiteMode.None,
                    SecurePolicy = CookieSecurePolicy.Always,
                    HttpOnly = true // Set HttpOnly to true to prevent client-side scripts from accessing the cookie
                };
            });
        });
    }
}