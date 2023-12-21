using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Webhooks;

internal class WebhooksWorker
(
    HttpListener listener,
    HttpRequestQueue httpRequestQueue,
    ILogger<WebhooksWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HTTP Listener is starting...");
        listener.Start();

        while (!stoppingToken.IsCancellationRequested && !httpRequestQueue.IsCompleted)
        {
            var ctx = await listener.GetContextAsync();

            if (stoppingToken.IsCancellationRequested || ctx is null)
            {
                break;
            }

            await httpRequestQueue.Enqueue(ctx, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HTTP Listener is stopping...");
        httpRequestQueue.Complete();
        return Task.CompletedTask;
    }
}

