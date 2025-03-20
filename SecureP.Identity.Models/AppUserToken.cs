using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserToken<TKey> : IdentityUserToken<TKey> where TKey : IEquatable<TKey>
{
    public virtual DateTime ExpiryDate { get; set; }

    [NotMapped]
    public AppUser<TKey> User { get; set; } = default!;
}