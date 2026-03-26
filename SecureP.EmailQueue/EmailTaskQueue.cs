using System.Threading.Channels;
using SecureP.Service.Abstraction;

namespace SecureP.EmailQueue;

public class EmailTaskQueue : IEmailTaskQueue
{
    private readonly Channel<SendEmailCommand> _queue = Channel.CreateBounded<SendEmailCommand>(10_000); // Create a bounded channel with a capacity of 10,000 messages

    public IAsyncEnumerable<SendEmailCommand> DequeueEmailsAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAllAsync(cancellationToken);
    }

    public async ValueTask EnqueueEmailAsync(SendEmailCommand command)
    {
        await _queue.Writer.WriteAsync(command);
    }
}
