namespace SecureP.Repository.Abstraction.Models;

public class ParkingRateValidationModel<TKey> where TKey : IEquatable<TKey>
{
    public TKey? Id { get; set; } = default!;
    public double? HourlyRate { get; set; } = default!;
    public double? DailyRate { get; set; } = default!;
    public double? MonthlyRate { get; set; } = default!;
}