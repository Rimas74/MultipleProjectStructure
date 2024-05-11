using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MultipleProjectStructure.BusinessLogic.Services.Interfaces;
using MultipleProjectStructure.Controllers;
using MultipleProjectStructure.Database.Entities;
using MultipleProjectStructure.Dtos;
using System.Drawing.Imaging;
using System.Drawing;
using Image = MultipleProjectStructure.Database.Entities.Image;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ControllersUnitTest
{
    public class ControllersTest
    {
        public readonly Mock<IImageService> _imageServiceMock;
        public readonly ImagesController _imagesController;
        private Image _seededImage;
        private Thumbnail _seededThumbnail;


        public ControllersTest()
        {
            _imageServiceMock = new Mock<IImageService>();
            _imagesController = new ImagesController(_imageServiceMock.Object);
            SetupMocks();
        }

        private void SetupMocks()
        {
            _seededImage = CreatedSeededImage();
            _seededThumbnail = CreatedSeedThumbnail(_seededImage);

            _imageServiceMock.Setup(s => s.GetImageAsync(1)).ReturnsAsync(_seededImage);
            _imageServiceMock.Setup(s => s.GetThumbnailAsync(1)).ReturnsAsync(_seededThumbnail);
        }

        private Image CreatedSeededImage()
        {
            return new Image
            {
                Id = 1,
                FileName = "testImage.jpg",
                Data = GenerateSampleJpegBytes()
            };
        }

        private Thumbnail CreatedSeedThumbnail(Image image)
        {
            return new Thumbnail
            {
                Id = 1,
                FileName = "thumb_testImage.jpg",
                Data = new byte[] { 10, 20, 30 },
                Image = image,
                ImageId = image.Id
            };
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

        private IFormFile CreateMockFormFile(string contentType, byte[] content)
        {
            var stream = new MemoryStream(content);
            var file = new Mock<IFormFile>();

            file.Setup(f => f.OpenReadStream()).Returns(stream);
            file.Setup(f => f.Length).Returns(content.Length);
            file.Setup(f => f.ContentType).Returns(contentType);
            file.Setup(f => f.FileName).Returns("testImage.jpg");
            return file.Object;
        }

        [Fact]
        public async Task Upload_ReturnsCreateResutl()
        {
            //Arrange
            var contentBytes = GenerateSampleJpegBytes();
            var formFile = CreateMockFormFile("image/jpeg", contentBytes);
            var imageUplaodDto = new ImageUploadDto()
            {
                FileName = "testImage",
                File = formFile
            };

            var newImage = new Image
            {
                Id = 1,
                FileName = "testImage.jpg",
                Data = contentBytes
            };

            _imageServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<Image>())).ReturnsAsync(newImage);

            //Act

            var result = await _imagesController.Upload(imageUplaodDto) as CreatedAtActionResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(newImage, result.Value);
        }


        [Fact]
        public async Task GetImage_ReturnsNotFound()
        {
            //Arrange

            _imageServiceMock.Setup(s => s.GetImageAsync(It.IsAny<int>())).ReturnsAsync((Image)null);

            //Act

            var result = await _imagesController.GetImage(1);

            //Assert

            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task GetImage_ReturnsImage()
        {
            //Arrange

            var result = await _imagesController.GetImage(1) as FileContentResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result.ContentType);
            Assert.Equal("testImage.jpg", result.FileDownloadName);
            Assert.Equal(GenerateSampleJpegBytes(), result.FileContents);
        }

        [Fact]
        public async Task GetThumbnail_ReturnsNotFound()
        {
            //Arrange

            _imageServiceMock.Setup(s => s.GetThumbnailAsync(It.IsAny<int>())).ReturnsAsync((Thumbnail)null);

            //Act 
            var result = await _imagesController.GetThumbnail(2);

            //Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task GetThumbnail_ReturnsThumbnail()
        {
            //Arrange

            var result = await _imagesController.GetThumbnail(1) as FileContentResult;

            //Assert

            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result.ContentType);
            Assert.Equal("thumb_testImage.jpg", result.FileDownloadName);
            Assert.Equal(_seededThumbnail.Data, result.FileContents);
        }
    }
}
