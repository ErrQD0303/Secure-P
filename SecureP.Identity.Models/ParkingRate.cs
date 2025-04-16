using System.ComponentModel.DataAnnotations;

namespace SecureP.Identity.Models;

public class ParkingRate<TKey> where TKey : IEquatable<TKey>
{
    [Key]
    public virtual TKey Id { get; set; } = default!;
    public virtual double HourlyRate { get; set; }
    public virtual double DailyRate { get; set; }
    public virtual double MonthlyRate { get; set; }
    public virtual string ConcurrencyStamp { get; set; } = default!;

    // Navigation properties
    public virtual ICollection<ParkingLocationRate<TKey>> ParkingLocationRates { get; set; } = default!;
}