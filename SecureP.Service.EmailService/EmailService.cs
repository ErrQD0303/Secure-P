using Microsoft.AspNetCore.Identity.UI.Services;
using SecureP.Service.Abstraction;
using SecureP.Shared;

namespace SecureP.Service.EmailService;

public class EmailService : IEmailService
{
    private readonly IEmailSender _otpEmailSender;

    public EmailService(IEmailSender otpEmailSender)
    {
        _otpEmailSender = otpEmailSender;
    }

    public async Task SendOTPEmailAsync(string userEmail, string otp)
    {
        await _otpEmailSender.SendEmailAsync(userEmail, AppConstants.AppEmail.OTPAppEmail.Subject, AppConstants.AppEmail.OTPAppEmail.GetHTTPMessage(otp));
    }
}
