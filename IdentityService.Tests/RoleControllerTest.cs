using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityService.Controllers;
using IdentityService.Services.Interface;
using IdentityService.Data.Entity;

namespace IdentityService.Tests.Controllers
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _roleServiceMock = new Mock<IRoleService>();
            _controller = new RoleController(_roleServiceMock.Object);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkWithList()
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };
            _roleServiceMock
                .Setup(s => s.GetAllRolesAsync())
                .ReturnsAsync(roles);

            var result = await _controller.GetRoles();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoles = Assert.IsType<List<Role>>(okResult.Value);
            Assert.Equal(2, returnedRoles.Count);
        }

        [Fact]
        public async Task GetRole_ReturnsNotFound_WhenRoleIsNull()
        {
            _roleServiceMock
                .Setup(s => s.GetRoleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Role)null);

            var result = await _controller.GetRole(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateRole_ReturnsCreated_WhenSuccessful()
        {
            var newRole = new Role { Id = 3, Name = "Editor" };
            _roleServiceMock
                .Setup(s => s.CreateRoleAsync("Editor"))
                .ReturnsAsync(newRole);

            var result = await _controller.CreateRole("Editor");

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var role = Assert.IsType<Role>(createdResult.Value);
            Assert.Equal(3, role.Id);
            Assert.Equal("Editor", role.Name);
            Assert.Equal(nameof(_controller.GetRole), createdResult.ActionName);
        }

        [Fact]
        public async Task UpdateRole_ReturnsNoContent_WhenSuccessful()
        {
            _roleServiceMock
                .Setup(s => s.UpdateRoleAsync(1, "SuperAdmin"))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateRole(1, "SuperAdmin");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ReturnsNoContent_WhenSuccessful()
        {
            _roleServiceMock
                .Setup(s => s.DeleteRoleAsync(1))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteRole(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ReturnsBadRequest_WhenExceptionThrown()
        {
            _roleServiceMock
                .Setup(s => s.DeleteRoleAsync(99))
                .ThrowsAsync(new System.Exception("Cannot delete role in use"));

            var result = await _controller.DeleteRole(99);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot delete role in use", badRequest.Value);
        }
    }
}
