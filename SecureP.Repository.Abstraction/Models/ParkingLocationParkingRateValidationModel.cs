namespace SecureP.Repository.Abstraction.Models;

public class ParkingLocationParkingRateValidationModel
{
    public double HourlyRate { get; set; } = default!;
    public double DailyRate { get; set; } = default!;
    public double MonthlyRate { get; set; } = default!;
}