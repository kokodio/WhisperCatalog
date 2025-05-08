using Microsoft.AspNetCore.Mvc;
using WhisperCatalog;
using WhisperCatalog.Model;
using WhisperCatalog.Services;

var builder = WebApplication.CreateSlimBuilder(args);

const string catalogDirectory = "Catalog";
Directory.CreateDirectory(catalogDirectory);

builder.ConfigureJson();
builder.ConfigureDependencies();

var app = builder.Build();

app.MapPost("/upload", async ([FromServices] BackgroundQueueService<WhisperTask> whisperQueue, IFormFile file) =>
    {
        var taskId = Guid.NewGuid();
        var uploadFolder = Path.Combine(catalogDirectory, taskId.ToString());

        Directory.CreateDirectory(uploadFolder);

        var filePath = Path.Combine(uploadFolder, file.FileName);

        if (!filePath.EndsWith(".wav")) return Results.BadRequest("Only .wav files are supported.");
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var task = new WhisperTask(taskId, filePath);
        whisperQueue.TryEnqueue(task);

        return Results.Ok();
    })
    .Accepts<IFormFile>("multipart/form-data")
    .DisableAntiforgery();


app.Run();
