using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Webhooks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebhooks(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var listener = new HttpListener();
            // TODO: external port can be set via docker
            listener.Prefixes.Add("http://+:80/");
            return listener;
        });


        services.AddHostedService<WebhooksWorker>();
        return services;
    }
}
