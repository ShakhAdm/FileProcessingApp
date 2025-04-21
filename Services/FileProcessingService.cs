using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FileProcessingApp.Data;
using FileProcessingApp.Models;

namespace FileProcessingApp.Services
{
    public class FileProcessingService : BackgroundService
    {
        private readonly IFileProcessingQueue _queue;
        private readonly IServiceProvider _services;
        private readonly ILogger<FileProcessingService> _logger;

        public FileProcessingService(IFileProcessingQueue queue, IServiceProvider services, ILogger<FileProcessingService> logger)
        {
            _queue = queue;
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FileProcessingService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var filePath))
                {
                    _logger.LogInformation($"Файл обработки: {filePath}");

                    try
                    {
                        if (!File.Exists(filePath))
                        {
                            _logger.LogWarning($"Файл не найден: {filePath}");
                            continue;
                        }

                        string content = await File.ReadAllTextAsync(filePath, stoppingToken);
                        int wordCount = content.Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
                        int lineCount = content.Split('\n').Length;
                        int symbolCount = content.Length;

                        using var scope = _services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var file = db.Files.FirstOrDefault(f => f.FileName == filePath);
                        var eventType = file == null ? FileEventType.Created : FileEventType.Updated;

                        if (file == null)
                        {
                            file = new FileRecord { FileName = filePath, Statistics = new List<FileStatistics>() };
                            db.Files.Add(file);
                        }

                        file.Statistics.Add(new FileStatistics
                        {
                            Event = eventType,
                            Timestamp = DateTime.UtcNow,
                            Words = wordCount,
                            Lines = lineCount,
                            Symbols = symbolCount
                        });

                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Обработанный файл: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Ошибка обработки файла: {filePath}");
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken); // если нет файлов, ждем
                }
            }
        }
    }
}
