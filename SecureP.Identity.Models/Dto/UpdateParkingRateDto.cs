namespace SecureP.Identity.Models.Dto;

public class UpdateParkingRateDto
{
    public double? HourlyRate { get; set; }
    public double? DailyRate { get; set; }
    public double? MonthlyRate { get; set; }
}