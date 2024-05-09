using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Database.Repositories.Interfaces;

namespace MultipleProjectStructure.BusinessLogic.Services.Interfaces
{
    public interface IImageService
    {
        Task<Image> UploadImageAsync(Image image);

        Task<Thumbnail> GetThumbnailAsync(int id);
        Task<Image> GetImageAsync(int id);
    }
}
