using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Webhooks;

internal class RequestQueueWorker
(
    HttpRequestQueue requestQueue,
    ILogger<RequestQueueWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && !requestQueue.IsCompleted)
        {
            var request = await requestQueue.Dequeue(stoppingToken);

            await ProcessRequest(request).ConfigureAwait(false);
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        if (context is null) return;
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        try
        {
            using var s = new StreamReader(context.Request.InputStream);
            var body = await s.ReadToEndAsync();

            logger.LogInformation("{body}", body);

            context.Response?.Close();
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Error occurred processing the HTTP request.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Process all unprocessed ones from the queue.
        await foreach (var request in requestQueue.DequeueAll(cancellationToken))
        {
            await ProcessRequest(request);
        }
    }
}
