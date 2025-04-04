using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppRole<TKey> : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    public AppRole() : base()
    {
    }
    public AppRole(string roleName) : base(roleName)
    {
    }
    public virtual ICollection<AppUserRole<TKey>> UserRoles { get; set; } = default!;
    public virtual ICollection<AppRoleClaim<TKey>> RoleClaims { get; set; } = default!;
}