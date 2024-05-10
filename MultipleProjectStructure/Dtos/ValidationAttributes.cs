using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace MultipleProjectStructure.Dtos
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null && file.Length > _maxFileSize)
                return new ValidationResult($"Maximum allowed file size is {_maxFileSize} bytes.");

            return ValidationResult.Success;
        }
    }
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                    return new ValidationResult($"Invalid file extension. Allowed extensions are: {string.Join(", ", _extensions)}");
            }

            return ValidationResult.Success;
        }
    }
}
