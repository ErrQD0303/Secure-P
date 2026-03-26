using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureP.Service.Abstraction;
using SecureP.Shared;

namespace SecureP.EmailQueue;

public class EmailBackgroundWorker(IEmailTaskQueue emailQueue, ILogger<EmailBackgroundWorker> logger, IEmailService emailService) : BackgroundService
{
    private readonly IEmailTaskQueue _emailQueue = emailQueue;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<EmailBackgroundWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email background worker is starting.");

        await foreach (var emailCommand in _emailQueue.DequeueEmailsAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Sending email to {email}.", emailCommand.Email);
                switch (emailCommand.EmailType)
                {
                    case AppConstants.SupportEmailType.ConfirmEmail:
                        await _emailService.SendConfirmationEmailAsync(emailCommand.Email, emailCommand.Object);
                        break;
                    case AppConstants.SupportEmailType.ForgotPassword:
                        await _emailService.SendForgotPasswordEmailAsync(emailCommand.Email, emailCommand.Object);
                        break;
                    case AppConstants.SupportEmailType.OTP:
                        await _emailService.SendOTPEmailAsync(emailCommand.Email, emailCommand.Object);
                        break;
                    default:
                        _logger.LogWarning("Unknown email type {emailType} for email {email}.", emailCommand.EmailType, emailCommand.Email);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {email}.", emailCommand.Email);
            }
        }
    }
}