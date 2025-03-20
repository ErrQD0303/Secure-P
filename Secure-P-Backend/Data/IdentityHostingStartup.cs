using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Service.TokenService.Extensions;

[assembly: HostingStartup(typeof(Secure_P_Backend.Data.IdentityHostingStartup))]
namespace Secure_P_Backend.Data;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddDbContext<AppDbContext<string>>(options =>
                options.UseSqlServer(
                    context.Configuration.GetConnectionString("SqlServer")));

            services.AddDefaultIdentity<AppUser<string>>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<AppDbContext<string>>()
                .AddDefaultTokenProviders();

            services.AddTokenService<string>(); // Add token service
        });
    }
}