using JAuth.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace JAuth.Tests
{
    public class DataControllerTests
    {
        private readonly Mock<ILogger<DataController>> _loggerMock;
        private readonly DataController _dataController;

        public DataControllerTests()
        {
            _loggerMock = new Mock<ILogger<DataController>>();
            _dataController = new DataController(_loggerMock.Object);
        }

        [Fact]
        public void GetPublicData_ReturnsOk()
        {
            // Act
            var result = _dataController.GetPublicData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("This is a public endpoint. No authentication required.", okResult.Value);
        }

        [Fact]
        public void GetProtectedData_ReturnsOk_WhenAuthorized()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
            }, "mock"));

            _dataController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = _dataController.GetProtectedData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("This is a protected endpoint. You are authenticated!", okResult.Value);
        }

        [Fact]
        public void GetProtectedData_ReturnsUnauthorized_WhenNotAuthorized()
        {
            // Arrange
            _dataController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // No user is set, simulating unauthorized access
            };

            // Act
            var result = _dataController.GetProtectedData();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }
    }
}
