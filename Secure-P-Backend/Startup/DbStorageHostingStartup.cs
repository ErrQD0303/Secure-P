[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.DbStorageHostingStartup))]
namespace Secure_P_Backend.Startup;

public class DbStorageHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddDbContext<AppDbContext<string>>(options =>
                options.UseSqlServer(
                    context.Configuration.GetConnectionString("SqlServer")));

            services.AddDefaultIdentity<AppUser<string>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddRoles<AppRole<string>>()
                .AddEntityFrameworkStores<AppDbContext<string>>()
                .AddDefaultTokenProviders();

            services.AddTokenService<string>(); // Add token service
            services.AddUserService<string>(); // Add user service
            services.AddParkingLocationService<string>(); // Add parking location service
            services.AddEmailService(context.Configuration);
            services.AddUploadService<string>();
        });
    }
}