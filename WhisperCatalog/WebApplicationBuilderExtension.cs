using Microsoft.AspNetCore.Http.Features;
using Whisper.net;
using WhisperCatalog.AudioProcessors;
using WhisperCatalog.Model;
using WhisperCatalog.Services;
using WhisperCatalog.SubtitleGenerators;

namespace WhisperCatalog;

public static class WebApplicationBuilderExtension
{
    public static void ConfigureJson(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options  =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
        
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1_000_000_000;
        });
        
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.MaxRequestBodySize = 1_000_000_000;
        });
    }
    
    public static void ConfigureDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<WhisperProcessor>(_ =>
        {
            var options = new WhisperOptions();
            builder.Configuration.Bind(options);

            var factory = WhisperFactory
                .FromPath(options.ModelPath)
                .CreateBuilder()
                .WithMaxLastTextTokens(0);

            if (options.EnableTranslation)
            {
                factory = factory.WithTranslate();
            }
    
            factory = factory.WithLanguage(!string.IsNullOrWhiteSpace(options.Language)
                ? options.Language
                : "auto");

            return factory.Build();
        });
        
        builder.Services.AddSingleton<BackgroundQueueService<WhisperTask>>();
        builder.Services.AddSingleton<IAudioProcessor, MonoWavProcessor>();
        builder.Services.AddSingleton<ISubtitleGenerator, SrtGenerator>();
        builder.Services.AddHostedService<WhisperService>();
    }
}