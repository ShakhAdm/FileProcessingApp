using FileProcessingApp.Data;
using FileProcessingApp.Models;
using FileProcessingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;


namespace FileProcessingApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileProcessingQueue _queue;
        private readonly ILogger<FilesController> _logger;

        public FilesController(AppDbContext dbContext, IFileProcessingQueue queue, ILogger<FilesController> logger)
        {
            _dbContext = dbContext;
            _queue = queue;
            _logger = logger;
        }

        // GET /api/files
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await _dbContext.Files
                .Include(f => f.Statistics)
                .ToListAsync();

            return Ok(files);
        }

        // POST /api/files/upload
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Загрузка текстового файла для обработки")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пустой.");

            if (Path.GetExtension(file.FileName).ToLower() != ".txt")
                return BadRequest("Допустимы только .txt файлы.");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsDir);

            var filePath = Path.Combine(uploadsDir, file.FileName);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Добавляем в очередь на обработку
            _queue.Enqueue(filePath);
            _logger.LogInformation($"Файл '{file.FileName}' загружен и добавлен в очередь.");

            return Ok(new { Message = "Файл загружен и ожидает обработки." });
        }

        // GET /api/files/{id}/statistics
        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetFileStatistics(int id)
        {
            var file = await _dbContext.Files
                .Include(f => f.Statistics)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
                return NotFound("Файл не найден.");

            return Ok(file.Statistics);
        }
    }
}
