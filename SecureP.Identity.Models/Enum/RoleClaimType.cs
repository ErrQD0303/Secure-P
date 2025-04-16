namespace SecureP.Identity.Models.Enum;

[Flags]
public enum RoleClaimType
{
    // General claims
    None = 0,
    AnonymousAccess = 1 << 0,

    // Identity Controller claims
    GetInfo = 1 << 1,
    ResendEmailConfirmation = 1 << 2,
    UpdateProfile = 1 << 3,
    ChangePassword = 1 << 4,
    CreateUser = 1 << 5,
    ReadUser = 1 << 6,
    UpdateUser = 1 << 7,
    DeleteUser = 1 << 8,

    // GDPR Controller claims
    // Get GDPR info will use AnonymousAccess claim

    // Token Controller claims
    // Get Token and Refresh Token will use AnonymousAccess claim

    // Upload Controller    
    ChangeAvatar = 1 << 9,

    // ParkingLocation Controller claims
    CreateParkingLocation = 1 << 10,
    ReadParkingLocation = 1 << 11,
    UpdateParkingLocation = 1 << 12,
    DeleteParkingLocation = 1 << 13,

    // ParkingRate Controller claims
    CreateParkingRate = 1 << 14,
    ReadParkingRate = 1 << 15,
    UpdateParkingRate = 1 << 16,
    DeleteParkingRate = 1 << 17,

    NormalUser = AnonymousAccess | GetInfo | ResendEmailConfirmation | UpdateProfile | ChangePassword | ChangeAvatar,
    Administrator = 1 << 18,
}