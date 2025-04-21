using Microsoft.AspNetCore.Authorization;
using SecureP.Identity.Models.Enum;

namespace SecureP.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public RoleClaimType Permission { get; }

    public PermissionRequirement(RoleClaimType permission)
    {
        Permission = permission;
    }
}
