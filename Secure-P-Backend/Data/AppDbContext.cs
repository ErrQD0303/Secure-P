using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Shared;

namespace Secure_P_Backend.Data;

public class AppDbContext<TKey> : IdentityDbContext<AppUser<TKey>, IdentityRole<TKey>, TKey, IdentityUserClaim<TKey>,
    IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
    where TKey : IEquatable<TKey>
{
    public AppDbContext(DbContextOptions<AppDbContext<TKey>> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(AppConstants.IdentityEntityFrameworkCore.DefaultSchema);
    }
}