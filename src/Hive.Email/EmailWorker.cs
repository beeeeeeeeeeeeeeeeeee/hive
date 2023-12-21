using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Email;

internal class EmailWorker
(
    SmtpServer.SmtpServer smtpServer,
    ILogger<EmailWorker> logger
) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SMTP Server is starting...");
        return smtpServer.StartAsync(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("SMTP Server is stopping...");
        smtpServer.Shutdown();
        return smtpServer.ShutdownTask;
    }
}
