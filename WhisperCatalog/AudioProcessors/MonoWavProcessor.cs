using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace WhisperCatalog.AudioProcessors;

public class MonoWavProcessor : IAudioProcessor
{
    public async Task<string> ProcessAsync(string inputPath, CancellationToken cancellationToken = default)
    {
        var monoAudioPath = Path.ChangeExtension(inputPath, "16k_mono.wav");

        await using var reader = new AudioFileReader(inputPath);
        ISampleProvider sample = reader;

        if (reader.WaveFormat.Channels > 1)
        {
            sample = new StereoToMonoSampleProvider(sample)
            {
                LeftVolume = 0.5f,
                RightVolume = 0.5f
            };
        }

        var resampler = new WdlResamplingSampleProvider(sample, 16000);
        WaveFileWriter.CreateWaveFile16(monoAudioPath, resampler);

        return monoAudioPath;
    }
}