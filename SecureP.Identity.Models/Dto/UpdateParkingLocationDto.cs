namespace SecureP.Identity.Models.Dto;

public class UpdateParkingLocationDto<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
    public virtual string Name { get; set; } = default!;
    public virtual string Address { get; set; } = default!;
    public virtual int Capacity { get; set; } = default!;
    public virtual int AvailableSpaces { get; set; } = default!;
    public virtual ParkingRate<TKey>? ParkingRate { get; set; } = default!;
}