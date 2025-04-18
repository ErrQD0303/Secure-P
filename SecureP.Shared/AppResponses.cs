namespace SecureP.Shared;

public class AppResponses
{

    public class UserLogoutResponses
    {
        public static string UserLoggedOut => "User logged out";
        public static string UserLogoutFailed => "User failed to logout";

        public static string UserNotFound => "User not found";
    }

    public class UserRegisterResponses
    {
        public static string UserRegistered => "User registered";
        public static string UserRegistrationFailed => "User failed to register";
    }
    public class GetUserInfoResponses
    {
        public static string UserNotFound => "User not found";
        public static string UserFound => "User found";
    }
    public class UserLoginResponses
    {
        public static string UserLoggedIn => "Login successful";
        public static string UserLoggedInWaitForOTP => "Login successful, waiting for OTP";
        public static string UserLoginFailed => "User logged in failed, Invalid email or password!";
    }
    public class EmailConfirmationResponses
    {
        public static string EmailConfirmed => "Email confirmed";
        public static string EmailNotConfirmed => "Email not confirmed";
        public static string EmailConfirmationFailed => "Email confirmation failed";
        public static string UserNotFound => "User has been deleted";
    }
    public class ResendEmailConfirmationResponses
    {
        public static string UserNotFound => "User has been deleted";
        public static string EmailSent => "Email sent";
        public static string EmailNotSent => "Email not sent";
        public static string EmailIsNotMatch => "Email is not match";
    }
    public class UpdateProfileResponses
    {
        public static string ProfileUpdated => "Profile updated";
        public static string ProfileNotUpdated => "Profile not updated";
        public static string UserNotFound => "User has been deleted";
    }
    public class UpdatePasswordResponses
    {
        public static string PasswordUpdated => "Password updated";
        public static string PasswordNotUpdated => "Password not updated";
        public static string UserNotFound => "User has been deleted";
        public static string OldPasswordNotMatch => "Old password is not match";
        public static string NewPasswordIsSameAsOldPassword => "New password is same as old password";
    }
    public class ForgotPasswordResponses
    {
        public static string UserNotFound => "User not found";
        public static string ForgotPasswordEmailSent => "Email sent";
        public static string EmailNotSent => "Email not sent";
        public static string UserDeleted => "User has been deleted";
        public static string UserNotConfirmed => "User not confirmed";
    }

    public class ResetPasswordResponses
    {
        public static string PasswordReset => "Password reset";
        public static string PasswordNotReset => "Some of your information is not correct";
        public static string UnexpectedError => "Unexpected error occurred";
    }

    public class CreateParkingLocationResponses
    {
        public static string ParkingLocationNotFound => "Parking location info not found";
        public static string ModelValidationFailed => "Parking location model validation failed";
        public static string ParkingLocationCreated => "Parking location created successfully";
    }

    public class GetParkingLocationResponses
    {
        public static string ParkingLocationNotFound => "Parking location not found";
        public static string ParkingLocationFound => "Parking location retrieved successfully.";
    }

    public class GetAllParkingLocationResponses
    {
        public static string NoParkingLocationsFound => "No parking locations found";
        public static string ParkingLocationsFound => "Parking locations retrieved successfully.";
    }

    public class UpdateParkingLocationResponses
    {
        public static string ParkingLocationUpdated => "Parking location updated successfully";
        public static string ParkingLocationNotUpdated => "Parking location not updated";
        public static string ParkingLocationNotFound => "Parking location not found";
        public static string ParkingLocationBodyNotFound => "Parking location body cannot be empty";
        public static string ModelValidationFailed => "Parking location model validation failed";
    }

    public class DeleteParkingLocationResponses
    {
        public static string ParkingLocationDeleted => "Parking location deleted successfully";
        public static string ParkingLocationNotDeleted => "Parking location not deleted";
    }

    public class GetAllParkingRateResponses
    {
        public static string NoParkingRatesFound => "No parking rates found";
        public static string ParkingRatesFound => "Parking rates retrieved successfully.";
    }

    public class GetParkingRateResponses
    {
        public static string ParkingRateFound => "Parking rate retrieved successfully.";
        public static string ParkingRateNotFound => "Parking rate not found";
    }

    public class CreateParkingRateResponses
    {
        public static string ParkingRateCreated => "Parking rate created successfully";
        public static string ParkingRateNotCreated => "Parking rate not created";
        public static string ModelValidationFailed => "Parking rate model validation failed";
        public static string ParkingRateBodyNotFound => "Parking rate body cannot be empty";
    }

    public class DeleteParkingRateResponses
    {
        public static string ParkingRateDeleted => "Parking rate deleted successfully";
        public static string ParkingRateNotDeleted => "Parking rate not deleted";
    }

    public class UpdateParkingRateResponses
    {
        public static string ParkingRateUpdated => "Parking rate updated successfully";
        public static string ParkingRateNotUpdated => "Parking rate not updated";
        public static string ParkingRateNotFound => "Parking rate not found";
        public static string ParkingRateBodyNotFound => "Parking rate body cannot be empty";
        public static string ModelValidationFailed => "Parking rate model validation failed";
    }
    public class CreateParkingZoneResponses
    {
        public static string ParkingZoneCreated => "Parking rate created successfully";
        public static string ParkingZoneNotCreated => "Parking rate not created";
        public static string ModelValidationFailed => "Parking rate model validation failed";
        public static string ParkingZoneBodyNotFound => "Parking rate body cannot be empty";
    }

    public class DeleteParkingZoneResponses
    {
        public static string ParkingZoneDeleted => "Parking zone deleted successfully";
        public static string ParkingZoneNotDeleted => "Parking zone not deleted";
    }

    public class UpdateParkingZoneResponses
    {
        public static string ParkingZoneUpdated => "Parking zone updated successfully";
        public static string ParkingZoneNotUpdated => "Parking zone not updated";
        public static string ParkingZoneNotFound => "Parking zone not found";
        public static string ParkingZoneBodyNotFound => "Parking zone body cannot be empty";
        public static string ModelValidationFailed => "Parking zone model validation failed";
    }

    public class GetParkingZoneResponses
    {
        public static string ParkingZoneFound => "Parking zone retrieved successfully.";
        public static string ParkingZoneNotFound => "Parking zone not found";
    }

    public class GetAllParkingZoneResponses
    {
        public static string NoParkingZonesFound => "No parking zones found";
        public static string ParkingZonesFound => "Parking zones retrieved successfully.";
    }
}