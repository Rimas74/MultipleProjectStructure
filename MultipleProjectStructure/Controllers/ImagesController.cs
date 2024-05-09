
using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Web;
using MultipleProjectStructure.BusinessLogic.Services.Interfaces;
using MultipleProjectStructure.BusinessLogic.Dtos;

namespace MultipleProjectStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService imageService;

        public ImagesController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpPost("Upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto imageUploadDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string originalFileName = imageUploadDto.File.FileName;
            string fileExtension = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                fileExtension = ".jpg";
            }
            string safeFileName = Path.GetFileNameWithoutExtension(imageUploadDto.FileName) + fileExtension;
            var image = new Database.Entities.Image
            {
                FileName = safeFileName,
                Data = await ReadFileBytes(imageUploadDto.File)
            };

            var imageUpload = await imageService.UploadImageAsync(image);
            return CreatedAtAction(nameof(GetImage), new { id = imageUpload.Id }, imageUpload);

        }

        private async Task<byte[]> ReadFileBytes(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await imageService.GetImageAsync(id);
            if (image == null) return NotFound();

            string safeFileName = Path.GetFileName(image.FileName);
            string fileExtension = Path.GetExtension(image.FileName);
            string contentType = GetContentTypeFromExtension(fileExtension);

            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(image.Data, contentType, safeFileName);
        }



        [HttpGet("thumbnail/{id}")]
        public async Task<IActionResult> GetThumbnail(int id)
        {
            var thumbnail = await imageService.GetThumbnailAsync(id);
            if (thumbnail == null) return NotFound();


            string safeFileName = Path.GetFileName(thumbnail.FileName);

            string fileExtension = Path.GetExtension(thumbnail.FileName);
            string contentType = GetContentTypeFromExtension(fileExtension);


            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/octet-stream";
            }


            return File(thumbnail.Data, contentType, safeFileName);
        }

        private string GetContentTypeFromExtension(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
