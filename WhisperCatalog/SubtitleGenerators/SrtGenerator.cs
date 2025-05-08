using Whisper.net;

namespace WhisperCatalog.SubtitleGenerators;

public class SrtGenerator(
    WhisperProcessor model
) : ISubtitleGenerator
{
    private string path = string.Empty;
    
    public async Task<string> GenerateAsync(FileStream audio, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new InvalidOperationException("Output path not set. Call TrySetPath before generating subtitles.");
        }
        
        await using var writer = new StreamWriter(path);

        var index = 1;
        await foreach (var segment in model.ProcessAsync(audio, cancellationToken))
        {
            writer.WriteLine(index++);
            await writer.WriteLineAsync($"{ToSrtTimestamp(segment.Start)} --> {ToSrtTimestamp(segment.End)}");
            await writer.WriteLineAsync(segment.Text);
            await writer.WriteLineAsync();
        }
        
        return path;
    }

    public bool TrySetPath(string directoryPath, string name)
    {
        var uncheckedPath = Path.Combine(directoryPath, name);
        
        if (!Directory.Exists(directoryPath)) return false;
        if (File.Exists(uncheckedPath)) return false;
        if (string.IsNullOrEmpty(name)) return false;
        
        path = Path.ChangeExtension(uncheckedPath, ".srt");
        return true;
    }

    private static string ToSrtTimestamp(TimeSpan time)
        => $"{time:hh\\:mm\\:ss},{time.Milliseconds:000}";
}