using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserRole<TKey> : IdentityUserRole<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual AppUser<TKey> User { get; set; } = default!;
    public virtual AppRole<TKey> Role { get; set; } = default!;
}