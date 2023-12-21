using Hive.Email.MessageStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using SmtpServer;
using SmtpServer.Storage;

namespace Hive.Email.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            const int blockSize = 1024;
            const int largeBufferMultiple = 1024 * 1024;
            const int maxBufferSize = 16 * largeBufferMultiple;

            // TODO: user defined Options?
            var recyclableMemoryStreamManagerOptions = new RecyclableMemoryStreamManager.Options
            {
                BlockSize = blockSize,
                LargeBufferMultiple = largeBufferMultiple,
                MaximumBufferSize = maxBufferSize,
#if DEBUG
                GenerateCallStacks = true,
#endif
                AggressiveBufferReturn = true,
                MaximumLargePoolFreeBytes = maxBufferSize * 4,
                MaximumSmallPoolFreeBytes = 100 * blockSize,
            };

            return new RecyclableMemoryStreamManager(recyclableMemoryStreamManagerOptions);
        });

        services.AddTransient<IMessageStore, InMemoryMessageStore>();

        services.AddSingleton(sp =>
        {
            // TODO: Get options from ENV vars
            var options = new SmtpServerOptionsBuilder()
            .ServerName("Hive SMTP Server")
            .Port(587)
            .Build();

            return new SmtpServer.SmtpServer(options, sp);
        });

        services.AddHostedService<EmailWorker>();

        return services;
    }
}
