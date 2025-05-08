using WhisperCatalog.AudioProcessors;
using WhisperCatalog.Model;
using WhisperCatalog.SubtitleGenerators;

namespace WhisperCatalog.Services;

public class WhisperService(
    BackgroundQueueService<WhisperTask> queue,
    IAudioProcessor audioProcessor,
    ISubtitleGenerator subtitleGenerator
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await queue.WaitToReadAsync(stoppingToken))
        {
            var task = await queue.DequeueAsync(stoppingToken);
            var monoAudioPath = await audioProcessor.ProcessAsync(task.AudioPath, stoppingToken);
            
            await using var audio = File.OpenRead(monoAudioPath);
            subtitleGenerator.TrySetPath(Path.GetDirectoryName(task.AudioPath)!, Path.GetFileNameWithoutExtension(task.AudioPath));
            
            await subtitleGenerator.GenerateAsync(audio, stoppingToken);
        }
    }
}