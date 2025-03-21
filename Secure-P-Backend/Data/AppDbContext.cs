using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Shared;

namespace Secure_P_Backend.Data;

public class AppDbContext<TKey> : IdentityDbContext<AppUser<TKey>, IdentityRole<TKey>, TKey, IdentityUserClaim<TKey>,
    IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, AppUserToken<TKey>>
    where TKey : IEquatable<TKey>
{
    public AppDbContext(DbContextOptions<AppDbContext<TKey>> options) : base(options)
    { }

    public DbSet<AppUserLicensePlate<TKey>> UserLicensePlates { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(AppConstants.IdentityEntityFrameworkCore.DefaultSchema);

        modelBuilder.Entity<AppUserToken<TKey>>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<AppUserLicensePlate<TKey>>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LicensePlateNumber });

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserLicensePlates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}