namespace SecureP.Identity.Models.Dto;

public class UpdateParkingZoneDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Capacity { get; set; } = default!;
    public int AvailableSpaces { get; set; } = default!;
    public string ConcurrencyStamp { get; set; } = default!;
}