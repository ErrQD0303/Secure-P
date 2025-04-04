using SecureP.Identity.Models;
using SecureP.Identity.Models.Enum;

namespace SecureP.Shared;

public class AppRoleClaimsDisplayName
{
    public virtual IDictionary<RoleClaimType, string> DisplayNames { get; set; } = new Dictionary<RoleClaimType, string>()
    {
        { RoleClaimType.None, "None" },
        { RoleClaimType.AnonymousAccess, "Anonymous User" },
        { RoleClaimType.GetInfo, "Get User Info" },
        { RoleClaimType.ResendEmailConfirmation, "Resend Email Confirmation" },
        { RoleClaimType.UpdateProfile, "Update Profile" },
        { RoleClaimType.ChangePassword, "Change Password" },
        { RoleClaimType.CreateUser, "Create User" },
        { RoleClaimType.ReadUser, "Read User" },
        { RoleClaimType.UpdateUser, "Update User" },
        { RoleClaimType.DeleteUser, "Delete User" },
        { RoleClaimType.ChangeAvatar, "Change Avatar" },
        { RoleClaimType.CreateParkingLocation, "Create Parking Location" },
        { RoleClaimType.ReadParkingLocation, "Read Parking Location" },
        { RoleClaimType.UpdateParkingLocation, "Update Parking Location" },
        { RoleClaimType.DeleteParkingLocation, "Delete Parking Location" },
        { RoleClaimType.Administrator, "Administrator" },
    };
}