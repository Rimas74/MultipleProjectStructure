using System.ComponentModel.DataAnnotations;
using MultipleProjectStructure.Database.Entities;
using Microsoft.AspNetCore.Http;
using MultipleProjectStructure.Dtos;

namespace MultipleProjectStructure.Dtos
{
    public class ImageUploadDto
    {
        public string FileName { get; set; }


        [Required]
        [MaxFileSize(5 * 1024 * 1024)] // 5 MB
        [AllowedExtensions(new[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile File { get; set; }
    }
}
