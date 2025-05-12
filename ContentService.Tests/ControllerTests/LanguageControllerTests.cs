using ContentService.Controllers;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContentService.Tests.ControllerTests
{
    public class LanguageControllerTests
    {
        private readonly Mock<ILanguageService> _languageServiceMock;
        private readonly LanguageController _controller;

        public LanguageControllerTests()
        {
            _languageServiceMock = new Mock<ILanguageService>();
            _controller = new LanguageController(_languageServiceMock.Object);
        }

        //[Fact]
        //public async Task GetAll_ShouldReturnOkResult_WithListOfLanguages()
        //{

        //    var expected = new List<LanguageResponseModel>
        //{
        //    new() { Id = 1, CultureCode = "en", DisplayName = "English" },
        //    new() { Id = 2, CultureCode = "hy", DisplayName = "Հայերեն" }
        //};

        //    _languageServiceMock.Setup(s => s.GetAllAsync())
        //        .ReturnsAsync(expected);

        //    var result = await _controller.GetAll();

        //    var okResult = result as OkObjectResult;
        //    okResult.Should().NotBeNull();
        //    okResult!.Value.Should().BeEquivalentTo(expected);
        //}

        //[Fact]
        //public async Task GetById_ShouldReturnLanguage_WhenExists()
        //{
        //    var lang = new LanguageResponseModel { Id = 1, CultureCode = "en", DisplayName = "English" };
        //    _languageServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(lang);

        //    var result = await _controller.GetById(1);

        //    var okResult = result as OkObjectResult;
        //    okResult.Should().NotBeNull();
        //    okResult!.Value.Should().BeEquivalentTo(lang);
        //}

        //[Fact]
        //public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        //{
        //    _languageServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((LanguageResponseModel?)null);

        //    var result = await _controller.GetById(99);

        //    result.Should().BeOfType<NotFoundResult>();
        //}
        //[Fact]
        //public async Task Create_ShouldReturnCreatedResult()
        //{
        //    var request = new LanguageRequestModel { CultureCode = "ru", DisplayName = "Русский" };
        //    var response = 5;

        //    _languageServiceMock.Setup(s => s.CreateAsync(request))
        //        .ReturnsAsync(response);

        //    var result = await _controller.Create(request);

        //    var created = result as CreatedAtActionResult;
        //    created.Should().NotBeNull();
        //    created!.Value.Should().BeEquivalentTo(response);
        //}

        [Fact]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            var updateModel = new LanguageUpdateModel
            {
                Id = 1,
                CultureCode = "en",
                DisplayName = "English Updated"
            };

            _languageServiceMock
                .Setup(s => s.UpdateAsync(updateModel))
                .ReturnsAsync(true);

            var result = await _controller.Update(updateModel);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenLanguageNotFound()
        {
            var updateModel = new LanguageUpdateModel
            {
                Id = 99,
                CultureCode = "xx",
                DisplayName = "Nonexistent"
            };

            _languageServiceMock
                .Setup(s => s.UpdateAsync(updateModel))
                .ThrowsAsync(new InvalidOperationException("Language with ID 99 was not found."));

            var result = await _controller.Update(updateModel);

            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.Value.Should().Be("Language with ID 99 was not found.");
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenUpdateReturnsFalse()
        {
            var updateModel = new LanguageUpdateModel
            {
                Id = 1,
                CultureCode = "en",
                DisplayName = "Test"
            };

            _languageServiceMock
                .Setup(s => s.UpdateAsync(updateModel))
                .ReturnsAsync(false);

            var result = await _controller.Update(updateModel);

            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Update failed.");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            _languageServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenNotExists()
        {
            _languageServiceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}