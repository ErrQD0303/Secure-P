using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Service.TokenService.Extensions;
using SecureP.Service.UserService.Extensions;
using SecureP.Data;

[assembly: HostingStartup(typeof(Secure_P_Backend.IdentityHostingStartup))]
namespace Secure_P_Backend;

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
            services.AddUserService<string>(); // Add user service
        });
    }
}