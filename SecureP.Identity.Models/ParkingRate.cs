namespace SecureP.Identity.Models;

public class ParkingRate<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey ParkingLocationId { get; set; } = default!;
    public double HourlyRate { get; set; }
    public double DailyRate { get; set; }
    public double MonthlyRate { get; set; }

    // Navigation properties
    public virtual ParkingLocation<TKey> ParkingLocation { get; set; } = default!;
}