using Microsoft.EntityFrameworkCore;
using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Database.Repositories.Interfaces;

namespace MultipleProjectStructure.Database.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ImageApiDbContext _context;
        public ImageRepository(ImageApiDbContext context)
        {
            _context = context;
        }
        public async Task<Image> AddImageAsync(Image image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<Thumbnail> AddThumbnailAsync(Thumbnail thumbnail)
        {
            _context.Thumbnails.Add(thumbnail);
            await _context.SaveChangesAsync();
            return thumbnail;
        }

        public async Task<Image> GetImageByIdAsync(int id)
        {
            return await _context.Images.Include(i => i.Thumbnail).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Thumbnail> GetThumbnailByIdAsync(int id)
        {
            return await _context.Thumbnails.FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
