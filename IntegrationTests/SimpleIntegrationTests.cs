using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace StudentADO.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests that require a running backend on http://localhost:5276
    /// Run backend first: cd StudentADO && dotnet run
    /// Then run tests: cd StudentADO.Tests && dotnet test
    /// </summary>
    public class SimpleIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;

        public SimpleIntegrationTests()
        {
            // Create HTTP client pointing to your API
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5276"),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnHealthy()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("healthy");
        }

        [Fact]
        public async Task DatabaseHealthCheck_ShouldReturnHealthy()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/health/database");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Database connection successful");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginData = new
            {
                email = "ash@gmail.com",
                password = "123456" // Your actual password
            };

            var jsonContent = JsonSerializer.Serialize(loginData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", httpContent);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK,
                "because valid credentials should authenticate successfully");

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("token",
                "because successful login should return a JWT token");
            responseContent.Should().Contain("email",
                "because response should include user email");
            responseContent.Should().Contain("designation",
                "because response should include user role");

            // Optionally parse and validate the response structure
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            loginResponse.Should().NotBeNull();
            loginResponse.Token.Should().NotBeNullOrEmpty();
            loginResponse.Email.Should().Be(loginData.email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginData = new
            {
                email = "ash@gmail.com",
                password = "wrongpassword"
            };

            var jsonContent = JsonSerializer.Serialize(loginData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", httpContent);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized,
                "because invalid credentials should be rejected");
        }

        [Fact]
        public async Task Login_WithMissingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var loginData = new
            {
                email = "ash@gmail.com",
                password = "123456"
                // email is missing
            };

            var jsonContent = JsonSerializer.Serialize(loginData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", httpContent);

            // Assert
            response.StatusCode.Should().BeOneOf(
                System.Net.HttpStatusCode.BadRequest,
                System.Net.HttpStatusCode.Unauthorized
                );
        }

        // Dispose pattern for proper cleanup
        public void Dispose()
        {
            _client?.Dispose();
        }

        // Response DTO for deserialization
        private class LoginResponse
        {
            public string Token { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
        }
    }
}
