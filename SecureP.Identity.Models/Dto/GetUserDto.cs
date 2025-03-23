namespace SecureP.Identity.Models.Dto;

public class GetUserDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public bool PhoneNumberConfirmed { get; set; } = default!;
    public DateTime DayOfBirth { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public bool TwoFactorEnabled { get; set; } = default!;
    public bool LockoutEnabled { get; set; } = default!;
    public DateTimeOffset? LockoutEnd { get; set; } = default!;
    public int AccessFailedCount { get; set; } = default!;
    public string PostCode { get; set; } = default!;
    public ICollection<GetUserTokenDto<TKey>> UserTokens { get; set; } = default!;
    public ICollection<string> LicensePlates { get; set; } = default!;
}
