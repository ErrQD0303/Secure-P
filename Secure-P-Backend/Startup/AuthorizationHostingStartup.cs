using SecureP.Identity.Models.Enum;

[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.AuthorizationHostingStartup))]
namespace Secure_P_Backend.Startup;

public class AuthorizationHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddAuthorization(options =>
            {
                foreach (var roleClaim in Enum.GetValues<RoleClaimType>().Cast<RoleClaimType>())
                {
                    options.AddPolicy(roleClaim.ToString(), policy =>
                    {
                        policy.RequireClaim(AppCustomClaims.Permission, roleClaim.ToString());
                    });
                }
            });
        });
    }
}