using Microsoft.EntityFrameworkCore;
using MultipleProjectStructure.Database;
using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Database.Repositories;
using MultipleProjectStructure.Database.Repositories.Interfaces;

namespace DatabaseUnitTest
{
    public class UnitTestDatabase
    {
        private ImageApiDbContext _context;
        private IImageRepository _imageRepository;

        public UnitTestDatabase()
        {
            var options = new DbContextOptionsBuilder<ImageApiDbContext>()
                 .UseInMemoryDatabase(databaseName: "TestDB")
                 .Options;

            _context = new ImageApiDbContext(options);
            _imageRepository = new ImageRepository(_context);

            ClearDatabase();
            SeedData();

        }

        private void ClearDatabase()
        {
            _context.Images.RemoveRange(_context.Images);
            _context.Thumbnails.RemoveRange(_context.Thumbnails);
            _context.SaveChanges();
        }

        private void SeedData()
        {
            var image = new Image
            {
                Id = 1,
                FileName = "testImage.jpg",
                Data = new byte[] { 1, 2, 3, 4 }

            };

            var thumbnail = new Thumbnail
            {
                Id = 1,
                FileName = "thumb_testImage.jpg",
                Data = new byte[] { 10, 20, 30 },
                Image = image,
            };

            image.Thumbnail = thumbnail;

            _context.Images.Add(image);
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddImageAsync_AddsImageSuccessfully()
        {
            var newImage = new Image
            {
                Id = 2,
                FileName = "newTestImage.jpg",
                Data = new byte[] { 5, 6, 7, 8 }
            };
            var addedNewImage = await _imageRepository.AddImageAsync(newImage);

            Assert.NotNull(addedNewImage);
            Assert.Equal("newTestImage.jpg", addedNewImage.FileName);
            Assert.NotEqual(0, addedNewImage.Id);
            Assert.Equal(new byte[] { 5, 6, 7, 8 }, addedNewImage.Data);

        }

        [Fact]

        public async Task AddThumbnailAsync_AddsThumbnailSuccsecfully()
        {
            var existingImage = await _imageRepository.GetImageByIdAsync(1);

            var newThumbnail = new Thumbnail
            {
                FileName = "newTestThumbnail.jpg",
                Data = new byte[] { 10, 25, 35 },
                //ImageId = existingImage.Id,
                Image = existingImage
            };

            var addedNewThumbnail = await _imageRepository.AddThumbnailAsync(newThumbnail);

            Assert.NotNull(newThumbnail);
            Assert.Equal("newTestThumbnail.jpg", addedNewThumbnail.FileName);
            Assert.Equal(existingImage.Id, addedNewThumbnail.Image.Id);
            Assert.Equal(new byte[] { 10, 25, 35 }, addedNewThumbnail.Data);
        }

        [Fact]
        public async Task GetImageByIdAsyn_ReturnsImageSuccessfully()
        {
            var image = await _imageRepository.GetImageByIdAsync(1);

            Assert.NotNull(image);
            Assert.Equal(1, image.Id);
            Assert.Equal("testImage.jpg", image.FileName);
            Assert.NotNull(image.Thumbnail);
            Assert.Equal("thumb_testImage.jpg", image.Thumbnail.FileName);

        }

        [Fact]
        public async Task GetThumbnailByIdAsync_ReturnsThumbnailSuccessfully()
        {
            var thumbnail = await _imageRepository.GetThumbnailByIdAsync(1);

            Assert.NotNull(thumbnail);
            Assert.Equal(1, thumbnail.Id);
            Assert.Equal(1, thumbnail.Image.Id);
            Assert.NotNull(thumbnail.Image);
            Assert.Equal("thumb_testImage.jpg", thumbnail.FileName);
        }
    }
}