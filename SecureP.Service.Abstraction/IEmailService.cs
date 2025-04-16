namespace SecureP.Service.Abstraction;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string url);
    Task SendForgotPasswordEmailAsync(string email, string url);
    Task SendOTPEmailAsync(string userEmail, string otp);
}