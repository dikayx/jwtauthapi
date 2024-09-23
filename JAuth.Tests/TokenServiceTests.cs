using JAuth.Api.Security;
using JAuth.UserEntityModels;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace JAuth.Tests
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["JwtSettings:SecretKey"]).Returns("SuperSecretKeyThatHasAtLeast32Bytes");
            _tokenService = new TokenService(_configurationMock.Object);
        }

        [Fact]
        public async Task GenerateJwtAsync_ShouldReturnValidToken()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser" };

            // Act
            var token = await _tokenService.GenerateJwtAsync(user);

            // Assert
            Assert.NotNull(token);

            // Verify the token structure using JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();
            var validatedToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(validatedToken);
            Assert.True(validatedToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public async Task GenerateJwtAsync_ShouldThrowExceptionForInvalidKey()
        {
            // Arrange
            _configurationMock.Setup(config => config["JwtSettings:SecretKey"]).Returns(string.Empty); // Invalid key
            var user = new ApplicationUser { UserName = "testuser" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _tokenService.GenerateJwtAsync(user));
        }
    }
}
