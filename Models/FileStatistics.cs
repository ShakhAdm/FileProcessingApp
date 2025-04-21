using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileProcessingApp.Models
{
    public class FileStatistics
    {
        public int Id { get; set; }
        public FileEventType Event { get; set; }
        public DateTime Timestamp { get; set; }
        public int Words { get; set; }
        public int Lines { get; set; }
        public int Symbols { get; set; }

        // Навигационное свойство
        public int FileRecordId { get; set; }
        public FileRecord FileRecord { get; set; } = null!;
    }
}
