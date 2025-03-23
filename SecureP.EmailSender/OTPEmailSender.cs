using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureP.Shared;
using SecureP.Shared.Configures;

namespace SecureP.EmailSender;

public class OTPEmailSender(ILogger<OTPEmailSender> logger, SmtpClient smtpClient, IOptions<AuthMessageSenderOptions> options) : EmailSender(logger, smtpClient, options)
{
    public override string EmailType { get; internal set; } = AppConstants.SupportEmailType.OTP;
}
