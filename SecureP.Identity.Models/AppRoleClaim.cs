using Microsoft.AspNetCore.Identity;
using SecureP.Identity.Models.Enum;

namespace SecureP.Identity.Models;

public class AppRoleClaim<TKey> : IdentityRoleClaim<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual new RoleClaimType ClaimValue { get; set; } = default!;

    public virtual AppRole<TKey> Role { get; set; } = default!;
}