using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SecureP.Identity.Models;

public class AppUserToken<TKey> : IdentityUserToken<TKey> where TKey : IEquatable<TKey>
{
    public virtual DateTime ExpiryDate { get; set; }

    public virtual AppUser<TKey> User { get; set; } = default!;
}