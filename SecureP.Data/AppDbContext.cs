using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Shared;
using SecureP.Shared.Mappers;

namespace SecureP.Data;

public class AppDbContext<TKey> : IdentityDbContext<AppUser<TKey>, AppRole<TKey>, TKey, AppUserClaim<TKey>,
    AppUserRole<TKey>, AppUserLogin<TKey>, AppRoleClaim<TKey>, AppUserToken<TKey>>
    where TKey : IEquatable<TKey>
{
    public AppDbContext(DbContextOptions<AppDbContext<TKey>> options) : base(options)
    { }

    public virtual DbSet<AppUserLicensePlate<TKey>> UserLicensePlates { get; set; } = default!;
    public virtual DbSet<ParkingLocation<TKey>> ParkingLocations { get; set; } = default!;
    public virtual DbSet<ParkingRate<TKey>> ParkingRates { get; set; } = default!;
    public virtual DbSet<ParkingZone<TKey>> ParkingZones { get; set; } = default!;
    public virtual DbSet<ParkingLocationRate<TKey>> ParkingLocationRates { get; set; } = default!;
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

            b.HasMany(uc => uc.UserClaims)
                .WithOne(u => u.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<AppRole<TKey>>(b =>
        {
        });

        builder.Entity<AppUserLogin<TKey>>(b =>
        {
            b.HasOne(ul => ul.User)
                .WithMany(u => u.UserLogins)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<AppUserToken<TKey>>(b =>
        {
            b.HasOne(d => d.User)
                .WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        builder.Entity<AppRoleClaim<TKey>>(b =>
        {
            b.HasOne(ur => ur.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Property(rc => rc.ClaimValue).HasConversion<int>();
        });

        builder.Entity<AppUserRole<TKey>>(b =>
        {
            b.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });

        builder.Entity<AppUserClaim<TKey>>(b =>
        {
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

            b.HasMany(pl => pl.ParkingLocationRates)
                .WithOne(plr => plr.ParkingLocation)
                .HasForeignKey(plr => plr.ParkingLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(pl => pl.ParkingZones)
                .WithOne(pz => pz.ParkingLocation)
                .HasForeignKey(pz => pz.ParkingLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            /* b.HasMany(pl => pl.UserParkingSubscriptions)
                .WithOne(ups => ups.ParkingLocation)
                .HasForeignKey(ups => ups.ParkingLocationId)
                .OnDelete(DeleteBehavior.SetNull); */

            b.Property(pl => pl.Name).HasMaxLength(256);
            b.Property(pl => pl.Address).HasMaxLength(512);
            b.Property(pl => pl.ConcurrencyStamp).IsConcurrencyToken();
        });

        builder.Entity<ParkingRate<TKey>>(b =>
        {
            b.HasKey(pr => pr.Id);

            b.HasMany(pr => pr.ParkingLocationRates)
                .WithOne(plr => plr.ParkingRate)
                .HasForeignKey(plr => plr.ParkingRateId)
                .OnDelete(DeleteBehavior.NoAction);

            b.ToTable("ParkingRates");

            b.Property(pr => pr.HourlyRate).HasDefaultValue(0.0);
            b.Property(pr => pr.DailyRate).HasDefaultValue(0.0);
            b.Property(pr => pr.MonthlyRate).HasDefaultValue(0.0);
            b.Property(pr => pr.ConcurrencyStamp).IsConcurrencyToken();
        });

        builder.Entity<ParkingLocationRate<TKey>>(b =>
        {
            b.HasKey(plr => plr.Id);

            b.HasIndex(plr => new { plr.ParkingLocationId, plr.ParkingRateId, plr.EffectiveFrom })
                .IsUnique();

            b.ToTable("ParkingLocationRates");

            b.Property(plr => plr.EffectiveTo).HasDefaultValue(null);
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

            b.HasOne(ups => ups.ParkingLocationRate)
                .WithMany(plr => plr.UserParkingSubscriptions)
                .HasForeignKey(ups => ups.ParkingLocationRateId)
                .OnDelete(DeleteBehavior.NoAction);

            b.Property(ups => ups.SubscriptionFee).HasDefaultValue(0.0);
            b.Property(ups => ups.ClampingFee).HasDefaultValue(0.0);
            b.Property(ups => ups.ChangeSignageFee).HasDefaultValue(0.0);
            b.Property(ups => ups.IsPaid).HasDefaultValue(false);
            // b.Property(ups => ups.PaymentDate).HasDefaultValue(null);
            b.Property(ups => ups.LicensePlate).HasMaxLength(256);
        });

        builder.Ignore<AppUserParkingSubscription<TKey, TKey, TKey, TKey>>();
    }
}