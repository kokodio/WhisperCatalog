namespace WhisperCatalog.Model;

public class WhisperOptions
{
    public string ModelPath { get; set; } = "ggml-base-q8_0.bin";
    public bool EnableTranslation { get; set; } = true;
    public string Language { get; set; } = "auto";
}