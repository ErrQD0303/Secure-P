using SecureP.Identity.Models.Enum;

namespace SecureP.Service.Abstraction;

public record SendEmailCommand(string Email, string Object, string EmailType);

public interface IEmailTaskQueue
{
    ValueTask EnqueueEmailAsync(SendEmailCommand command);
    IAsyncEnumerable<SendEmailCommand> DequeueEmailsAsync(CancellationToken cancellationToken);
}