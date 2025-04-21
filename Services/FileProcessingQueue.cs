using System.Collections.Concurrent;

namespace FileProcessingApp.Services
{
    public class FileProcessingQueue : IFileProcessingQueue
    {
        private readonly ConcurrentQueue<string> _queue = new();

        public void Enqueue(string filePath)
        {
            _queue.Enqueue(filePath);
        }

        public bool TryDequeue(out string filePath)
        {
            return _queue.TryDequeue(out filePath);
        }

        public int Count => _queue.Count;
    }
}
