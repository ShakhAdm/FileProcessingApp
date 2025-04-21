namespace FileProcessingApp.Services
{
    public interface IFileProcessingQueue
    {
        void Enqueue(string filePath);
        bool TryDequeue(out string filePath);
        int Count { get; }
    }
}
