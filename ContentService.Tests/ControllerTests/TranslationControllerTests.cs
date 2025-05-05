using ContentService.Controllers;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentService.Tests.ControllerTests
{
    public class TranslationControllerTests
    {
        private readonly Mock<ITranslationService> _service;
        private readonly TranslationController _controller;

        public TranslationControllerTests()
        {
            _service = new Mock<ITranslationService>();
            _controller = new TranslationController(_service.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TranslationResponseModel>());
            var result = await _controller.GetAll();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_IfNull()
        {
            _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((TranslationResponseModel?)null);
            var result = await _controller.GetById(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var model = new TranslationRequestModel { LocalizationId = 1, LanguageId = 1, Value = "Hello" };
            var response = new TranslationResponseModel { Id = 1, LocalizationId = 1, LanguageId = 1, Value = "Hello" };
            _service.Setup(s => s.CreateAsync(model)).ReturnsAsync(response);
            var result = await _controller.Create(model);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenNull()
        {
            var model = new TranslationUpdateModel { Id = 1, LocalizationId = 1, LanguageId = 1, Value = "Update" };
            _service.Setup(s => s.UpdateAsync(model)).ReturnsAsync((TranslationResponseModel?)null);
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
