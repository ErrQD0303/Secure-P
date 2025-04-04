using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserClaim<TKey> : IdentityUserClaim<TKey> where TKey : IEquatable<TKey>
{
    public virtual AppUser<TKey> User { get; set; } = default!;
}