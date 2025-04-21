using System.Collections.Generic;

namespace FileProcessingApp.Models
{
    public class FileRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public List<FileStatistics> Statistics { get; set; } = new();
    }
}
