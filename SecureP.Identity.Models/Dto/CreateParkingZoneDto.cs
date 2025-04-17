namespace SecureP.Identity.Models.Dto;

public class CreateParkingZoneDto<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
    public virtual string Name { get; set; } = default!;
    public virtual int Capacity { get; set; } = default!;
    public virtual int AvailableSpaces { get; set; } = default!;
}