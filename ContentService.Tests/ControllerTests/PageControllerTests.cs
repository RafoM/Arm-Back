using ContentService.Controllers;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContentService.Tests.ControllerTests
{
    public class PageControllerTests
    {
        private readonly Mock<IPageService> _service;
        private readonly PageController _controller;

        public PageControllerTests()
        {
            _service = new Mock<IPageService>();
            _controller = new PageController(_service.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PageResponseModel>());
            var result = await _controller.GetAll();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_IfNull()
        {
            _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((PageResponseModel?)null);
            var result = await _controller.GetById(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var model = new PageRequestModel { Name = "Test", DisplayName = "Test" };
            var response = new PageResponseModel { Id = 1, Name = "Test", DisplayName = "Test" };
            _service.Setup(s => s.CreateAsync(model)).ReturnsAsync(response);
            var result = await _controller.Create(model);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            var model = new PageUpdateModel
            {
                Id = 1,
                Name = "Updated Name",
                DisplayName = "Updated Display"
            };

            var response = new PageResponseModel
            {
                Id = 1,
                Name = "Updated Name",
                DisplayName = "Updated Display"
            };

            _service.Setup(s => s.UpdateAsync(model)).ReturnsAsync(response);

            var result = await _controller.Update(model);

            result.Should().BeOfType<OkObjectResult>();

            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenPageNotFound()
        {
            var model = new PageUpdateModel
            {
                Id = 99,
                Name = "Missing",
                DisplayName = "Doesn't Exist"
            };

            _service.Setup(s => s.UpdateAsync(model)).ReturnsAsync((PageResponseModel?)null);

            var result = await _controller.Update(model);

            result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            _service.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
