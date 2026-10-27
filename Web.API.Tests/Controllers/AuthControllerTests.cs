using Application.Mediart.Commands.Auth;
using Application.TDOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.API.Controllers;
using Xunit;

namespace Web.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<ISender> _mockMediator;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<ISender>();
            _authController = new AuthController(_mockMediator.Object);
        }

        #region Register Tests

        [Fact]
        public async Task Register_WithValidCommand_ReturnsOkResultWithAuthResponse()
        {
            // Arrange
            var registerCommand = new RegisterCommand(
                FirstName: "John",
                LastName: "Doe",
                Email: "john@example.com",
                Password: "SecurePassword123"
            );

            var expectedResponse = new AuthResponseDto(
                JwtToken: "test_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(1)
            );

            _mockMediator
                .Setup(m => m.Send(registerCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.CreateTask(registerCommand, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedResponse = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.JwtToken, returnedResponse.JwtToken);
            Assert.Equal(expectedResponse.TokenExpiration, returnedResponse.TokenExpiration);
            _mockMediator.Verify(m => m.Send(registerCommand, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Register_WithValidCommand_SendsCommandToMediator()
        {
            // Arrange
            var registerCommand = new RegisterCommand(
                FirstName: "Jane",
                LastName: "Smith",
                Email: "jane@example.com",
                Password: "AnotherPassword456"
            );

            var expectedResponse = new AuthResponseDto(
                JwtToken: "another_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(1)
            );

            _mockMediator
                .Setup(m => m.Send(registerCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _authController.CreateTask(registerCommand, CancellationToken.None);

            // Assert
            _mockMediator.Verify(
                m => m.Send(It.Is<RegisterCommand>(c =>
                    c.FirstName == "Jane" &&
                    c.LastName == "Smith" &&
                    c.Email == "jane@example.com" &&
                    c.Password == "AnotherPassword456"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Register_WithCancellationToken_PassesCancellationTokenToMediator()
        {
            // Arrange
            var registerCommand = new RegisterCommand(
                FirstName: "Bob",
                LastName: "Johnson",
                Email: "bob@example.com",
                Password: "BobPassword789"
            );

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var expectedResponse = new AuthResponseDto(
                JwtToken: "bob_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(1)
            );

            _mockMediator
                .Setup(m => m.Send(registerCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _authController.CreateTask(registerCommand, cancellationToken);

            // Assert
            _mockMediator.Verify(
                m => m.Send(registerCommand, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Register_WhenMediatorThrowsException_PropagatesException()
        {
            // Arrange
            var registerCommand = new RegisterCommand(
                FirstName: "Invalid",
                LastName: "User",
                Email: "invalid@example.com",
                Password: "pass"
            );

            _mockMediator
                .Setup(m => m.Send(registerCommand, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Registration failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authController.CreateTask(registerCommand, CancellationToken.None));
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCommand_ReturnsOkResultWithAuthResponse()
        {
            // Arrange
            var loginCommand = new LoginCommand(
                Email: "john@example.com",
                Password: "SecurePassword123"
            );

            var expectedResponse = new AuthResponseDto(
                JwtToken: "test_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(7)
            );

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.Login(loginCommand, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedResponse = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(expectedResponse.JwtToken, returnedResponse.JwtToken);
            Assert.Equal(expectedResponse.TokenExpiration, returnedResponse.TokenExpiration);
            _mockMediator.Verify(m => m.Send(loginCommand, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_WithValidCommand_SendsCommandToMediator()
        {
            // Arrange
            var loginCommand = new LoginCommand(
                Email: "jane@example.com",
                Password: "AnotherPassword456"
            );

            var expectedResponse = new AuthResponseDto(
                JwtToken: "another_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(7)
            );

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _authController.Login(loginCommand, CancellationToken.None);

            // Assert
            _mockMediator.Verify(
                m => m.Send(It.Is<LoginCommand>(c =>
                    c.Email == "jane@example.com" &&
                    c.Password == "AnotherPassword456"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithCancellationToken_PassesCancellationTokenToMediator()
        {
            // Arrange
            var loginCommand = new LoginCommand(
                Email: "bob@example.com",
                Password: "BobPassword789"
            );

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var expectedResponse = new AuthResponseDto(
                JwtToken: "bob_jwt_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(7)
            );

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            await _authController.Login(loginCommand, cancellationToken);

            // Assert
            _mockMediator.Verify(
                m => m.Send(loginCommand, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_MediatorThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginCommand = new LoginCommand(
                Email: "invalid@example.com",
                Password: "wrongpassword"
            );

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authController.Login(loginCommand, CancellationToken.None));
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task Login_WhenMediatorThrowsException_PropagatesException()
        {
            // Arrange
            var loginCommand = new LoginCommand(
                Email: "error@example.com",
                Password: "password"
            );

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Login failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authController.Login(loginCommand, CancellationToken.None));
        }

        #endregion

        #region Integration-style Tests

        [Fact]
        public async Task RegisterAndLogin_SequentialCalls_BothReturnValidResponses()
        {
            // Arrange
            var registerCommand = new RegisterCommand(
                FirstName: "Test",
                LastName: "User",
                Email: "test@example.com",
                Password: "TestPassword123"
            );

            var loginCommand = new LoginCommand(
                Email: "test@example.com",
                Password: "TestPassword123"
            );

            var registerResponse = new AuthResponseDto(
                JwtToken: "register_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(1)
            );

            var loginResponse = new AuthResponseDto(
                JwtToken: "login_token",
                TokenExpiration: DateTimeOffset.UtcNow.AddDays(7)
            );

            _mockMediator
                .Setup(m => m.Send(registerCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(registerResponse);

            _mockMediator
                .Setup(m => m.Send(loginCommand, It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginResponse);

            // Act
            var registerResult = await _authController.CreateTask(registerCommand, CancellationToken.None);
            var loginResult = await _authController.Login(loginCommand, CancellationToken.None);

            // Assert
            var registerOkResult = Assert.IsType<OkObjectResult>(registerResult.Result);
            var loginOkResult = Assert.IsType<OkObjectResult>(loginResult.Result);

            var registerReturnedResponse = Assert.IsType<AuthResponseDto>(registerOkResult.Value);
            var loginReturnedResponse = Assert.IsType<AuthResponseDto>(loginOkResult.Value);

            Assert.Equal("register_token", registerReturnedResponse.JwtToken);
            Assert.Equal("login_token", loginReturnedResponse.JwtToken);

            _mockMediator.Verify(
                m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockMediator.Verify(
                m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion
    }
}
