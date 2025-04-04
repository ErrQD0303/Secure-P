using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserLogin<TKey> : IdentityUserLogin<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual AppUser<TKey> User { get; set; } = default!;
}