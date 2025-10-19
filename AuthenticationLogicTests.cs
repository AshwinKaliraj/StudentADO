using Moq;
using StudentADO.BusinessLogic;
using StudentADO.DTOs;
using StudentADO.Helpers;
using StudentADO.Models;
using StudentADO.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StudentADO.Tests
{
    public class AuthenticationLogicTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<JwtHelper> _mockJwtHelper;
        private readonly AuthenticationLogic _authLogic;

        public AuthenticationLogicTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockJwtHelper = new Mock<JwtHelper>("testkey123456789012345678901234", "TestIssuer", "TestAudience", 60);
            _authLogic = new AuthenticationLogic(_mockRepo.Object, _mockJwtHelper.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Name = "John Doe",
                Email = "john@test.com",
                Password = "password123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Designation = "Teacher"
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(false);
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);

            // Act
            var (success, message, userId) = await _authLogic.RegisterAsync(registerDto);

            // Assert
            Assert.True(success);
            Assert.Equal("Registration successful", message);
            Assert.Equal(1, userId);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Name = "John Doe",
                Email = "existing@test.com",
                Password = "password123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Designation = "Student"
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(true);

            // Act
            var (success, message, userId) = await _authLogic.RegisterAsync(registerDto);

            // Assert
            Assert.False(success);
            Assert.Equal("Email already exists", message);
            Assert.Equal(0, userId);
        }

        [Fact]
        public async Task RegisterAsync_InvalidDesignation_ReturnsFailure()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Name = "John Doe",
                Email = "john@test.com",
                Password = "password123",
                DateOfBirth = new DateTime(1990, 1, 1),
                Designation = "Admin" // Invalid designation
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(registerDto.Email)).ReturnsAsync(false);

            // Act
            var (success, message, userId) = await _authLogic.RegisterAsync(registerDto);

            // Assert
            Assert.False(success);
            Assert.Contains("Teacher", message);
            Assert.Contains("Student", message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithToken()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@test.com",
                PasswordHash = PasswordHelper.HashPassword("password123"),
                Designation = "Teacher",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _mockJwtHelper.Setup(j => j.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                          .Returns("fake-jwt-token");

            // Act
            var (success, message, response) = await _authLogic.LoginAsync(loginDto);

            // Assert
            Assert.True(success);
            Assert.Equal("Login successful", message);
            Assert.NotNull(response);
            Assert.Equal("fake-jwt-token", response.Token);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "nonexistent@test.com",
                Password = "password123"
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

            // Act
            var (success, message, response) = await _authLogic.LoginAsync(loginDto);

            // Assert
            Assert.False(success);
            Assert.Equal("Invalid email or password", message);
            Assert.Null(response);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@test.com",
                PasswordHash = PasswordHelper.HashPassword("correctpassword"),
                Designation = "Student",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            // Act
            var (success, message, response) = await _authLogic.LoginAsync(loginDto);

            // Assert
            Assert.False(success);
            Assert.Equal("Invalid email or password", message);
            Assert.Null(response);
        }
    }
}
