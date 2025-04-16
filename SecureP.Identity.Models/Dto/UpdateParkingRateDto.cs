namespace SecureP.Identity.Models.Dto;

public class UpdateParkingRateDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public double? HourlyRate { get; set; }
    public double? DailyRate { get; set; }
    public double? MonthlyRate { get; set; }
    public string ConcurrencyStamp { get; set; } = default!;
}