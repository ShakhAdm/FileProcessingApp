using Microsoft.EntityFrameworkCore;
using FileProcessingApp.Models;

namespace FileProcessingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FileRecord> Files => Set<FileRecord>();
        public DbSet<FileStatistics> Statistics => Set<FileStatistics>();
    }
}
