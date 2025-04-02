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
}