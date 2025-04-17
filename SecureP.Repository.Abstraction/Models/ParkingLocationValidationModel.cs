namespace SecureP.Repository.Abstraction.Models;

public class ParkingLocationValidationModel<TKey> where TKey : IEquatable<TKey>
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public IEnumerable<ParkingLocationParkingZoneValidationModel> ParkingZones { get; set; } = default!;
    public TKey? ParkingRateId { get; set; } = default!;
}