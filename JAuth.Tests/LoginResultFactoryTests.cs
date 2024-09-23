using JAuth.Api.Factories;

namespace JAuth.Tests
{
    public class LoginResultFactoryTests
    {
        [Fact]
        public void Create_ReturnsSuccessLoginResult()
        {
            // Arrange
            bool success = true;
            string message = "Login successful";
            string token = "mockJwtToken";

            // Act
            var result = LoginResultFactory.Create(success, message, token);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal("mockJwtToken", result.Token);
        }

        [Fact]
        public void Create_ReturnsFailureLoginResult_WithoutToken()
        {
            // Arrange
            bool success = false;
            string message = "Login failed";

            // Act
            var result = LoginResultFactory.Create(success, message);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Login failed", result.Message);
            Assert.Null(result.Token);
        }
    }
}
