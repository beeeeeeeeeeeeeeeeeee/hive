using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Email;

internal class EmailWorker : BackgroundService
{
    private readonly SmtpServer.SmtpServer _smtpServer;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(SmtpServer.SmtpServer smtpServer, ILogger<EmailWorker> logger)
    {
        _smtpServer = smtpServer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return  _smtpServer.StartAsync(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _smtpServer.Shutdown();
        return _smtpServer.ShutdownTask;
    }
}
