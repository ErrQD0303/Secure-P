namespace SecureP.Repository.Abstraction.Models;

public class ParkingLocationParkingZoneValidationModel<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Capacity { get; set; } = default!;
    public int AvailableSpaces { get; set; } = default!;
}