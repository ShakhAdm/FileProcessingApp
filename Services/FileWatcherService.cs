using Microsoft.Extensions.Options;
using FileProcessingApp.Configuration;

namespace FileProcessingApp.Services
{
    public class FileWatcherService : BackgroundService
    {
        private readonly IFileProcessingQueue _queue;
        private readonly FileProcessingOptions _options;
        private FileSystemWatcher? _watcher;

        public FileWatcherService(IFileProcessingQueue queue, IOptions<FileProcessingOptions> options)
        {
            _queue = queue;
            _options = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _watcher = new FileSystemWatcher(_options.WatchDirectory, "*.txt")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;

            return Task.CompletedTask;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Кладем в очередь файл
            _queue.Enqueue(e.FullPath);
        }

        public override void Dispose()
        {
            base.Dispose();
            _watcher?.Dispose();
        }
    }
}
