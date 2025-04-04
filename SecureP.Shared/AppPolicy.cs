using SecureP.Identity.Models;

namespace SecureP.Shared;

public class AppPolicy
{
    public const string None = "None";
    public const string AnonymousAccess = "AnonymousAccess";
    public const string GetInfo = "GetInfo";
    public const string ResendEmailConfirmation = "ResendEmailConfirmation";
    public const string UpdateProfile = "UpdateProfile";
    public const string ChangePassword = "ChangePassword";
    public const string CreateUser = "CreateUser";
    public const string ReadUser = "ReadUser";
    public const string UpdateUser = "UpdateUser";
    public const string DeleteUser = "DeleteUser";
    public const string ChangeAvatar = "ChangeAvatar";
    public const string CreateParkingLocation = "CreateParkingLocation";
    public const string ReadParkingLocation = "ReadParkingLocation";
    public const string UpdateParkingLocation = "UpdateParkingLocation";
    public const string DeleteParkingLocation = "DeleteParkingLocation";
    public const string NormalUser = "NormalUser";
    public const string Administrator = "Administrator";
}