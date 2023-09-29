using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Webhooks;

internal class WebhooksWorker : BackgroundService
{
    private readonly HttpListener _listener;
    private readonly ILogger<WebhooksWorker> _logger;

    public WebhooksWorker(HttpListener listener, ILogger<WebhooksWorker> logger)
    {
        _listener = listener;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            var ctx = await _listener.GetContextAsync();

            if (stoppingToken.IsCancellationRequested)
            {
                continue;
            }

            //var json = await JsonSerializer.DeserializeAsync(ctx.Request.InputStream, _serializerOptions, cancellationToken: stoppingToken);
            var json = await new StreamReader(ctx.Request.InputStream).ReadToEndAsync(stoppingToken);

            _logger.LogInformation("json body: {json}", json);

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.Close();
        }
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Close();
        return Task.CompletedTask;
    }
}

