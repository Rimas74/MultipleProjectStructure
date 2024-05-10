using Moq;
using MultipleProjectStructure.BusinessLogic.Services;
using MultipleProjectStructure.BusinessLogic.Services.Interfaces;
using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Database.Repositories.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = MultipleProjectStructure.Database.Entities.Image;

namespace BusinessLogicUnitTest
{
    public class UnitTestService
    {
        private readonly IImageService _imageService;
        private readonly Mock<IImageRepository> _imageRepositoryMock;
        private readonly Image _seededImage;
        private readonly Thumbnail _seededThumbnail;



        public UnitTestService()
        {
            _imageRepositoryMock = new Mock<IImageRepository>();

            _seededImage = new Image
            {
                Id = 1,
                FileName = "testImage.jpg",
                Data = GenerateSampleJpegBytes()

            };
            _seededThumbnail = new Thumbnail
            {
                Id = 1,
                FileName = "thumb_testImage.jpg",
                Data = new byte[] { 10, 20, 30 },
                Image = _seededImage,
                ImageId = _seededImage.Id
            };

            _imageRepositoryMock.Setup(repo => repo.GetImageByIdAsync(1)).ReturnsAsync(_seededImage);
            _imageRepositoryMock.Setup(repo => repo.GetThumbnailByIdAsync(1)).ReturnsAsync(_seededThumbnail);

            _imageService = new ImageService(_imageRepositoryMock.Object);


        }

        private static byte[] GenerateSampleJpegBytes()
        {
            using Bitmap bitmap = new Bitmap(100, 100);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Blue);

            using MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }


        [Fact]
        public async Task GetImageAsync_ReturnsCorrectImage()
        {
            //Act
            var result = await _imageService.GetImageAsync(1);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("testImage.jpg", result.FileName);

        }

        [Fact]
        public async Task GetThumbnailAsync_ReturnsCorrectThumbnail()
        {
            //Act
            var result = await _imageService.GetThumbnailAsync(1);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("thumb_testImage.jpg", result.FileName);

        }

        [Fact]
        public async Task UploadImageAsync_ReturnsCorrectImageAndThumbnail()
        {
            var jpegData = GenerateSampleJpegBytes();

            var newImage = new Image
            {
                FileName = "newTestmage.jpg",
                Data = jpegData
            };

            var savedImage = new Image
            {
                Id = 2,
                FileName = "newTestImage.jpg",
                Data = jpegData
            };
            var newThumbnail = new Thumbnail
            {
                Id = 2,
                ImageId = 2,
                FileName = "thumb_newTestImage.jpg",
                Image = savedImage,
                Data = new byte[] { 10, 20, 30 }
            };

            _imageRepositoryMock.Setup(repo => repo.AddImageAsync(It.IsAny<Image>()))
                .ReturnsAsync(savedImage);
            _imageRepositoryMock.Setup(repo => repo.AddThumbnailAsync(It.IsAny<Thumbnail>()))
                .ReturnsAsync(newThumbnail);

            var result = await _imageService.UploadImageAsync(newImage);

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("newTestImage.jpg", result.FileName);

            _imageRepositoryMock.Verify(repo => repo.AddImageAsync(newImage), Times.Once());

            _imageRepositoryMock.Verify(repo => repo.AddThumbnailAsync(It.Is<Thumbnail>(t =>
                t.ImageId == newThumbnail.ImageId &&
                t.Image.Id == newThumbnail.ImageId &&
                t.FileName == newThumbnail.FileName)), Times.Once);

        }
    }
}