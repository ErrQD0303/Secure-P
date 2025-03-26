using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureP.EmailSender;
using SecureP.Service.Abstraction;
using SecureP.Shared.Configures;

namespace SecureP.Service.EmailService.Extensions;

public static class EmailServiceExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(services => new SmtpClient(configuration["Smtp:Host"]) // host: smtp.gmail.com
        {
            Port = int.Parse(configuration["Smtp:Port"] ?? throw new InvalidOperationException("Port not found")),
            Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
            EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"] ?? true.ToString())
        });

        services.Configure<AuthMessageSenderOptions>(configuration);

        services.AddTransient<IEmailService, EmailService>(services =>
        {
            var otpEmailSender = new OTPEmailSender(
                services.GetRequiredService<ILogger<OTPEmailSender>>(),
                services.GetRequiredService<SmtpClient>(),
                services.GetRequiredService<IOptions<AuthMessageSenderOptions>>());

            var confirmEmailSender = new ConfirmEmailSender(
                services.GetRequiredService<ILogger<ConfirmEmailSender>>(),
                services.GetRequiredService<SmtpClient>(),
                services.GetRequiredService<IOptions<AuthMessageSenderOptions>>());

            return new EmailService(otpEmailSender, confirmEmailSender);
        });

        return services;
    }
}