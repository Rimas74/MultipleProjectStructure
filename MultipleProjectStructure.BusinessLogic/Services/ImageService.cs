using MultipleProjectStructure.BusinessLogic.Services.Interfaces;
using MultipleProjectStructure.Database.Repositories.Interfaces;
using MultipleProjectStructure.Database;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using MultipleProjectStructure.Database.Entities;

namespace MultipleProjectStructure.BusinessLogic.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<Database.Entities.Image> GetImageAsync(int id) => await _imageRepository.GetImageByIdAsync(id);

        public async Task<Thumbnail> GetThumbnailAsync(int id) => await _imageRepository.GetThumbnailByIdAsync(id);

        public async Task<Database.Entities.Image> UploadImageAsync(Database.Entities.Image image)
        {
            var savedImage = await _imageRepository.AddImageAsync(image);

            var thumbnailData = CreateThumbnail(image.Data, 100, 100);
            var thumbnail = new Thumbnail
            {
                ImageId = savedImage.Id,
                Image = savedImage,
                FileName = $"thumb_{savedImage.FileName}",
                Data = thumbnailData
            };

            await _imageRepository.AddThumbnailAsync(thumbnail);
            return savedImage;
        }


        private byte[] CreateThumbnail(byte[] imageData, int width, int height)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("Image data cannot be null or empty.", nameof(imageData));
            }
            try
            {
                using var ms = new MemoryStream(imageData);
                using var img = System.Drawing.Image.FromStream(ms);

                using var thumb = img.GetThumbnailImage(width, height, null, nint.Zero);

                using var thumbStream = new MemoryStream();
                thumb.Save(thumbStream, ImageFormat.Jpeg);

                return thumbStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error creating thumbnail. The image data may be invalid., ex");
            }

        }


    }
}
