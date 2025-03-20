using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUser<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    public virtual ICollection<AppUserToken<TKey>> UserTokens { get; set; } = default!;
}