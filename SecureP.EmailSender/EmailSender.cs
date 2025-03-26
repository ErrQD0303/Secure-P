using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureP.Shared;
using SecureP.Shared.Configures;

namespace SecureP.EmailSender;

public class EmailSender : IEmailSender
{
    protected readonly ILogger<EmailSender> _logger;
    protected readonly SmtpClient _smtpClient;
    public virtual string EmailType { get; internal set; } = AppConstants.SupportEmailType.Default;

    public EmailSender(ILogger<EmailSender> logger, SmtpClient smtpClient, IOptions<AuthMessageSenderOptions> options)
    {
        _logger = logger;
        _smtpClient = smtpClient;
        Options = options.Value;
    }

    public AuthMessageSenderOptions Options { get; }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrEmpty(Options.SenderEmail))
        {
            throw new InvalidOperationException("Sender Email is missing");
        }

        await ExecuteAsync(subject, htmlMessage, email);
    }

    protected virtual async Task ExecuteAsync(string subject, string message, string toEmail)
    {
        var mailMessage = new MailMessage
        {
            Sender = new MailAddress(Options.SenderEmail!),
            From = new MailAddress(Options.SenderEmail!),
            To = { toEmail },
            Subject = subject,
            Body = message,
            SubjectEncoding = System.Text.Encoding.UTF8,
            BodyEncoding = System.Text.Encoding.UTF8,
            IsBodyHtml = true,
        };

        mailMessage.ReplyToList.Add(Options.SenderEmail!);

        _logger.LogInformation("Sending {emailType} email to {toEmail}", EmailType, toEmail);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}