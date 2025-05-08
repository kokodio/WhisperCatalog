namespace WhisperCatalog.SubtitleGenerators;

public interface ISubtitleGenerator
{
    public Task<string> GenerateAsync(FileStream audio, CancellationToken cancellationToken = default);
    public bool TrySetPath(string directoryPath, string name);
}