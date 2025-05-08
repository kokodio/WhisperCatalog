using System.Threading.Channels;

namespace WhisperCatalog.Services;

public class BackgroundQueueService<T>
{
    public BackgroundQueueService()
    {
        var channel = Channel.CreateUnbounded<T>();
        writer = channel.Writer;
        reader = channel.Reader;
    }

    private readonly ChannelWriter<T> writer;
    private readonly ChannelReader<T> reader;

    public bool TryEnqueue(T item)
    {
        return writer.TryWrite(item);
    }
    
    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return reader.ReadAsync(cancellationToken);
    }
    
    public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
    {
        return reader.WaitToReadAsync(cancellationToken);
    }
}