using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessingApp.Models
{
    public class UploadFileRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
