using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityService.Controllers;
using IdentityService.Services.Interface;
using IdentityService.Models.RequestModels;

namespace IdentityService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkWithTokens_WhenRegistrationSuccessful()
        {
            // Arrange
            var request = new RegisterRequestModel
            {
                // Fill out with test data if needed
                Email = "test@example.com",
                Password = "Password123"
            };

            // Mock the service to return a known token pair
            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(("accessTokenValue", "refreshTokenValue"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("accessTokenValue", (string)response.accessToken);
            Assert.Equal("refreshTokenValue", (string)response.refreshToken);
        }

        [Fact]
        public async Task Login_ReturnsOkWithTokens_WhenLoginSuccessful()
        {
            // Arrange
            var request = new LoginRequestModel { Email = "test@example.com", Password = "Password123" };
            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(("accessToken123", "refreshToken123"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("accessToken123", (string)response.accessToken);
            Assert.Equal("refreshToken123", (string)response.refreshToken);
        }

        [Fact]
        public async Task GoogleLogin_ReturnsOkWithTokens_WhenSuccessful()
        {
            // Arrange
            var idToken = "dummyGoogleIdToken";
            _authServiceMock
                .Setup(s => s.GoogleLoginAsync(idToken))
                .ReturnsAsync(("accessTokenG", "refreshTokenG"));

            // Act
            var result = await _controller.GoogleLogin(idToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("accessTokenG", (string)response.accessToken);
            Assert.Equal("refreshTokenG", (string)response.refreshToken);
        }

        [Fact]
        public async Task GoogleRegister_ReturnsOkWithTokens_WhenSuccessful()
        {
            // Arrange
            var idToken = "dummyGoogleIdToken";
            _authServiceMock
                .Setup(s => s.GoogleRegistrationAsync(idToken))
                .ReturnsAsync(("accessTokenGR", "refreshTokenGR"));

            // Act
            var result = await _controller.GoogleRegister(idToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("accessTokenGR", (string)response.accessToken);
            Assert.Equal("refreshTokenGR", (string)response.refreshToken);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOkWithTokens_WhenTokenIsValid()
        {
            // Arrange
            var oldRefresh = "oldRefreshToken";
            _authServiceMock
                .Setup(s => s.RefreshTokenAsync(oldRefresh))
                .ReturnsAsync(("newAccess", "newRefresh"));

            // Act
            var result = await _controller.RefreshToken(oldRefresh);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("newAccess", (string)response.accessToken);
            Assert.Equal("newRefresh", (string)response.newRefreshToken);
        }

        [Fact]
        public async Task Logout_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var refreshToken = "someRefreshToken";

            // No return for LogoutAsync, just a task
            _authServiceMock
                .Setup(s => s.LogoutAsync(refreshToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout(refreshToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Equal("Logged out", (string)response.message);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOk_WhenCalled()
        {
            // Arrange
            var email = "forgot@example.com";
            _authServiceMock
                .Setup(s => s.ForgotPasswordAsync(email))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Contains("reset link", (string)response.message);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenCalled()
        {
            // Arrange
            var token = "resetToken";
            var newPassword = "NewPassword123";
            _authServiceMock
                .Setup(s => s.ResetPasswordAsync(token, newPassword))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ResetPassword(token, newPassword);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic? response = okResult.Value;
            Assert.Contains("Password has been reset", (string)response.message);
        }
    }
}
