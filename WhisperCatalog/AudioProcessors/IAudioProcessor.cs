namespace WhisperCatalog.AudioProcessors;

public interface IAudioProcessor
{
    public Task<string> ProcessAsync(string inputPath, CancellationToken cancellationToken);
}