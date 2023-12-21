using System.Net;
using System.Threading.Channels;

namespace Hive.Webhooks;

internal class HttpRequestQueue
{
    private readonly Channel<HttpListenerContext> _channel = Channel.CreateBounded<HttpListenerContext>(new BoundedChannelOptions(1500)
    {
        SingleWriter = true,
        FullMode = BoundedChannelFullMode.Wait
    });

    private volatile bool _completed = false;

    public ValueTask Enqueue(HttpListenerContext context, CancellationToken ct) => _channel.Writer.WriteAsync(context, ct);

    public ValueTask<HttpListenerContext> Dequeue(CancellationToken ct) => _channel.Reader.ReadAsync(ct);

    public IAsyncEnumerable<HttpListenerContext> DequeueAll(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);

    public void Complete()
    {
        _channel.Writer.Complete();
        _completed = true;
    }

    public bool IsCompleted => _completed;
}
