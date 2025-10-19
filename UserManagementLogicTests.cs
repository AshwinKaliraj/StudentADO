using Moq;
using StudentADO.BusinessLogic;
using StudentADO.DTOs;
using StudentADO.Models;
using StudentADO.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StudentADO.Tests
{
    public class UserManagementLogicTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserManagementLogic _userLogic;

        public UserManagementLogicTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userLogic = new UserManagementLogic(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Name = "User 1", Email = "user1@test.com", Designation = "Teacher", DateOfBirth = DateTime.Now, CreatedAt = DateTime.UtcNow },
                new User { UserId = 2, Name = "User 2", Email = "user2@test.com", Designation = "Student", DateOfBirth = DateTime.Now, CreatedAt = DateTime.UtcNow }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userLogic.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@test.com",
                Designation = "Teacher",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreatedAt = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userLogic.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User", result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _userLogic.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "Old Name",
                Email = "test@test.com",
                Designation = "Student",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var updateDto = new UpdateUserDTO
            {
                Name = "New Name",
                DateOfBirth = new DateTime(1991, 2, 2)
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var (success, message) = await _userLogic.UpdateUserAsync(1, updateDto);

            // Assert
            Assert.True(success);
            Assert.Equal("User updated successfully", message);
        }

        [Fact]
        public async Task UpdateUserAsync_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            var updateDto = new UpdateUserDTO
            {
                Name = "New Name"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var (success, message) = await _userLogic.UpdateUserAsync(999, updateDto);

            // Assert
            Assert.False(success);
            Assert.Equal("User not found", message);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsSuccess()
        {
            // Arrange
            var user = new User { UserId = 1, Name = "To Delete", Email = "delete@test.com", Designation = "Student", DateOfBirth = DateTime.Now };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var (success, message) = await _userLogic.DeleteUserAsync(1);

            // Assert
            Assert.True(success);
            Assert.Equal("User deleted successfully", message);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var (success, message) = await _userLogic.DeleteUserAsync(999);

            // Assert
            Assert.False(success);
            Assert.Equal("User not found", message);
        }
    }
}
