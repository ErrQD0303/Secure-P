using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserLicensePlate<TKey> where TKey : IEquatable<TKey>
{
    [PersonalData]
    public virtual string LicensePlateNumber { get; set; } = default!;
    [PersonalData]
    public virtual TKey UserId { get; set; } = default!;
    public virtual AppUser<TKey> User { get; set; } = default!;
}