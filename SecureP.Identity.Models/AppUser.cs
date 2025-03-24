using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUser<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    [Required]
    public virtual string FullName { get; set; } = default!;
    [Required, DataType(DataType.Date)]
    public virtual DateTime DayOfBirth { get; set; } = default!;
    [Required]
    public virtual string Country { get; set; } = default!;
    public virtual string City { get; set; } = default!;
    [Required]
    public virtual string AddressLine1 { get; set; } = default!;
    public virtual string? AddressLine2 { get; set; } = default!;
    public virtual string PostCode { get; set; } = default!;

    public virtual ICollection<AppUserToken<TKey>> UserTokens
    { get; set; } = default!;
    public virtual ICollection<AppUserLicensePlate<TKey>> UserLicensePlates
    { get; set; } = default!;
}