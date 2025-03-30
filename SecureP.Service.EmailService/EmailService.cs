using Microsoft.AspNetCore.Identity.UI.Services;
using SecureP.Service.Abstraction;
using SecureP.Shared;

namespace SecureP.Service.EmailService;

public class EmailService : IEmailService
{
    private readonly IEmailSender _otpEmailSender;
    private readonly IEmailSender _confirmEmailSender;
    private readonly IEmailSender _forgotPasswordEmailSender;

    public EmailService(IEmailSender otpEmailSender, IEmailSender confirmEmailSender, IEmailSender forgotPasswordEmailSender)
    {
        _otpEmailSender = otpEmailSender;
        _confirmEmailSender = confirmEmailSender;
        _forgotPasswordEmailSender = forgotPasswordEmailSender;
    }

    public async Task SendConfirmationEmailAsync(string email, string url)
    {
        await _confirmEmailSender.SendEmailAsync(email, AppConstants.AppEmail.ConfirmAppEmail.Subject, AppConstants.AppEmail.ConfirmAppEmail.GetHTTPMessage(url));
    }

    public async Task SendOTPEmailAsync(string userEmail, string otp)
    {
        await _otpEmailSender.SendEmailAsync(userEmail, AppConstants.AppEmail.OTPAppEmail.Subject, AppConstants.AppEmail.OTPAppEmail.GetHTTPMessage(otp));
    }

    public async Task SendForgotPasswordEmailAsync(string email, string url)
    {
        await _forgotPasswordEmailSender.SendEmailAsync(email, AppConstants.AppEmail.ForgotPasswordAppEmail.Subject, AppConstants.AppEmail.ForgotPasswordAppEmail.GetHTTPMessage(url));
    }
}
