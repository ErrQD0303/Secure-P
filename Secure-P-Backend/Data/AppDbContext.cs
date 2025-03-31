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

    public virtual DbSet<AppUserLicensePlate<TKey>> UserLicensePlates { get; set; } = default!;
    public virtual DbSet<ParkingLocation<TKey>> ParkingLocations { get; set; } = default!;
    public virtual DbSet<ParkingRate<TKey>> ParkingRates { get; set; } = default!;
    public virtual DbSet<ParkingZone<TKey>> ParkingZones { get; set; } = default!;
    public virtual DbSet<AppUserParkingSubscription<TKey, TKey, TKey, TKey>> UserParkingSubscriptions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(AppConstants.IdentityEntityFrameworkCore.DefaultSchema);

        builder.Entity<AppUser<TKey>>(b =>
        {
            b.HasMany(u => u.UserParkingSubscriptions)
                .WithOne(ups => ups.User)
                .HasForeignKey(ups => ups.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<AppUserToken<TKey>>(b =>
        {
            b.HasOne(d => d.User)
                .WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<AppUserLicensePlate<TKey>>(b =>
        {
            b.HasKey(e => new { e.UserId, e.LicensePlateNumber });

            b.HasOne(d => d.User)
                .WithMany(p => p.UserLicensePlates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<ParkingLocation<TKey>>(b =>
        {
            b.ToTable("ParkingLocations");

            b.HasOne(pl => pl.ParkingRate)
                .WithOne(pr => pr.ParkingLocation)
                .HasForeignKey<ParkingRate<TKey>>(pr => pr.ParkingLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(pl => pl.ParkingZones)
                .WithOne(pz => pz.ParkingLocation)
                .HasForeignKey(pz => pz.ParkingLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(pl => pl.UserParkingSubscriptions)
                .WithOne(ups => ups.ParkingLocation)
                .HasForeignKey(ups => ups.ParkingLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            b.Property(pl => pl.Name).HasMaxLength(256);
            b.Property(pl => pl.Address).HasMaxLength(512);
            b.Property(pl => pl.AvailableSpaces).HasDefaultValue(0);
            b.Property(pl => pl.Capacity).HasDefaultValue(0);
        });

        builder.Entity<ParkingRate<TKey>>(b =>
        {
            b.HasKey(pr => pr.ParkingLocationId);

            b.ToTable("ParkingRates");

            b.Property(pr => pr.HourlyRate).HasDefaultValue(0.0);
            b.Property(pr => pr.DailyRate).HasDefaultValue(0.0);
            b.Property(pr => pr.MonthlyRate).HasDefaultValue(0.0);
        });

        builder.Entity<ParkingZone<TKey>>(b =>
        {
            b.ToTable("ParkingZones");

            b.HasMany(pz => pz.UserParkingSubscriptions)
                .WithOne(ups => ups.ParkingZone)
                .HasForeignKey(ups => ups.ParkingZoneId)
                .OnDelete(DeleteBehavior.SetNull);

            b.Property(pz => pz.Name).HasMaxLength(256);
            b.Property(pz => pz.AvailableSpaces).HasDefaultValue(0);
            b.Property(pz => pz.Capacity).HasDefaultValue(0);
        });

        builder.Entity<AppUserParkingSubscription<TKey>>(b =>
        {
            b.ToTable("UserParkingSubscriptions");

            /* b.HasOne(ups => ups.ParkingLocation)
                .WithMany(pl => pl.UserParkingSubscriptions)
                .HasForeignKey(ups => ups.ParkingLocationId)
                .OnDelete(DeleteBehavior.Cascade); */

            // b.Property(ups => ups.StartDate).HasDefaultValue(DateTime.UtcNow);
            // b.Property(ups => ups.EndDate).HasDefaultValue(DateTime.UtcNow.AddMonths(1));
            b.Property(ups => ups.SubscriptionFee).HasDefaultValue(0.0);
            b.Property(ups => ups.ClampingFee).HasDefaultValue(0.0);
            b.Property(ups => ups.ChangeSignageFee).HasDefaultValue(0.0);
            b.Property(ups => ups.IsPaid).HasDefaultValue(false);
            // b.Property(ups => ups.PaymentDate).HasDefaultValue(null);
            b.Property(ups => ups.LicensePlate).HasMaxLength(256);
        });
    }
}