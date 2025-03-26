namespace SecureP.Identity.Models.Dto;

public class NewUserDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public DateTime DayOfBirth { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; } = default!;
    public string PostCode { get; set; } = default!;
    public ICollection<string> LicensePlates { get; set; } = default!;
    public string? Avatar { get; set; }
}
