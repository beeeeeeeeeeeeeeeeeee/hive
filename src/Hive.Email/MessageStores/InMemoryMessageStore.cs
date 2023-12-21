using System.Buffers;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace Hive.Email.MessageStores;

internal class InMemoryMessageStore
(
    RecyclableMemoryStreamManager streamManager,
    ILogger<InMemoryMessageStore> logger
) : MessageStore
{
    public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        await using var stream = streamManager.GetStream("");

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory))
        {
            await stream.WriteAsync(memory, cancellationToken);
        }

        stream.Position = 0;

        var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

        logger.LogInformation("From: {fromEmail} - To: {toEmail} - Body: {body}", message.From.ToString(), message.To.ToString(), message.TextBody);

        return SmtpResponse.Ok;
    }
}
