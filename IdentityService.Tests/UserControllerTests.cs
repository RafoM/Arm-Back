using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using IdentityService.Controllers;
using IdentityService.Services.Interface;
using IdentityService.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using IdentityService.Models.ResponseModels;
using IdentityService.Data.Entity;

namespace IdentityService.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUserInfo_ReturnsOkWithUser_WhenFound()
        {
            var userId = Guid.NewGuid();
            var userInfo = new { Id = userId, FirstName = "John", LastName = "Doe" };

           _userServiceMock
    .Setup(s => s.GetUserInfoAsync(userId))
    .ReturnsAsync(new UserInfoResponseModel 
    { 
        Id = userId, 
        FirstName = "John", 
        LastName = "Doe", 
        Email = "john.doe@example.com",
        Country = "USA",
        PhoneNumber = "555-1234",
        TelegramUserName = "johndoe",
        ProfileImageUrl = "https://example.com/profile.jpg",
        RoleName = "User"
    });

            var result = await _controller.GetUserInfo(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userInfo, okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkWithList()
        {
            var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                PasswordHash = "hashedPassword1",
                Country = "USA",
                PhoneNumber = "1234567890",
                TelegramUserName = "alice123",
                ReferralCode = "REF123",
                ProfileImageUrl = "https://example.com/alice.jpg",
                RoleId = 2,
                Role = new Role { Id = 2, Name = "User" },
                IsGmailAccount = false,
                CreatedDate = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Jones",
                Email = "bob@example.com",
                PasswordHash = "hashedPassword2",
                Country = "Canada",
                PhoneNumber = "0987654321",
                TelegramUserName = "bob456",
                ReferralCode = "REF456",
                ProfileImageUrl = null,
                RoleId = 1,
                Role = new Role { Id = 1, Name = "Admin" },
                IsGmailAccount = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }
        };

            _userServiceMock
                .Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(users);

            var result = await _controller.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task UpdateUserInfo_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var requestModel = new UserInfoUpdateRequestModel
            {
                FirstName = "John",
                LastName = "Updated"
            };

            _userServiceMock
                .Setup(s => s.UpdateUserInfoAsync(userId, requestModel))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateUserInfo(userId, requestModel);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("User info successfully updated", (string)response.message);
        }

        [Fact]
        public async Task UpdateUserRole_ReturnsOkWithUpdatedUser_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var newRoleId = 2;
            var updatedUser = new { Id = userId, RoleId = newRoleId };

            _userServiceMock
    .Setup(s => s.GetUserInfoAsync(userId))
    .ReturnsAsync(new UserInfoResponseModel
    {
        Id = userId,
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        Country = "USA",
        PhoneNumber = "555-1234",
        TelegramUserName = "johndoe",
        ProfileImageUrl = "https://example.com/profile.jpg",
        RoleName = "User"
    });
            var result = await _controller.UpdateUserRole(userId, newRoleId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedUser, okResult.Value);
        }

        [Fact]
        public async Task UploadProfileImage_ReturnsOkWithImageUrl_WhenSuccessful()
        {
            var fakeImageUrl = "https://s3.amazonaws.com/test/user123.jpg";
            var fileMock = new Mock<IFormFile>();

            var content = "Fake Image Content";
            var fileName = "testimage.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            _userServiceMock
                .Setup(s => s.UpdateUserProfileImageAsync(It.IsAny<Guid>(), fileMock.Object))
                .ReturnsAsync(fakeImageUrl);

            var result = await _controller.UploadProfileImage(fileMock.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal(fakeImageUrl, (string)response.imageUrl);
        }
    }
}
