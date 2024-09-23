using JAuth.Api.Controllers;
using JAuth.Api.Data;
using JAuth.Api.Repositories;
using JAuth.UserEntityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace JAuth.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Set up mock configuration for JWT secret
            _configurationMock.SetupGet(c => c["JwtSettings:SecretKey"]).Returns("ThisIsASecretKeyForTestingWithAtLeast32bytes");

            _accountController = new AccountController(_userManagerMock.Object, _userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest { Email = "existinguser@test.com", Name = "Test User", Password = "Password123!" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registerRequest.Email)).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _accountController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Register_NewUserCreated_ReturnsOk()
        {
            // Arrange
            var registerRequest = new RegisterRequest { Email = "newuser@test.com", Name = "New User", Password = "Password123!" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registerRequest.Email)).ReturnsAsync((ApplicationUser)null);
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(registerRequest)).ReturnsAsync(new ApplicationUser { Email = registerRequest.Email, UserName = registerRequest.Email });

            // Act
            var result = await _accountController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "invaliduser@test.com", Password = "WrongPassword" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _accountController.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithJwt()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "validuser@test.com", Password = "CorrectPassword" };
            var user = new ApplicationUser { Email = loginRequest.Email, UserName = loginRequest.Email };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);

            // Act
            var result = await _accountController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var loginResult = Assert.IsType<LoginResult>(okResult.Value);
            Assert.True(loginResult.Success);
            Assert.NotNull(loginResult.Token);
        }

        [Fact]
        public async Task Get_UserExists_ReturnsOk()
        {
            // Arrange
            var email = "existinguser@test.com";
            var user = new ApplicationUser { Email = email, UserName = email };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _accountController.Get(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task Get_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var email = "nonexistentuser@test.com";
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _accountController.Get(email);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_UserExists_ReturnsOk()
        {
            // Arrange
            var updateRequest = new UpdateAccountRequest { Email = "existinguser@test.com", Name = "Updated Name", CurrentPassword = "CurrentPassword", NewPassword = "NewPassword123!" };
            var user = new ApplicationUser { Email = updateRequest.Email, UserName = updateRequest.Email, Name = "Old Name" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(updateRequest.Email)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(updateRequest, user)).ReturnsAsync(user);

            // Act
            var result = await _accountController.Update(updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Update_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var updateRequest = new UpdateAccountRequest { Email = "nonexistentuser@test.com" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(updateRequest.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _accountController.Update(updateRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Delete_UserExists_ReturnsOk()
        {
            // Arrange
            var deleteRequest = new DeleteAccountRequest { Email = "existinguser@test.com" };
            var user = new ApplicationUser { Email = deleteRequest.Email, UserName = deleteRequest.Email };
            _userManagerMock.Setup(um => um.FindByEmailAsync(deleteRequest.Email)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(user)).ReturnsAsync(user);

            // Act
            var result = await _accountController.Delete(deleteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Delete_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var deleteRequest = new DeleteAccountRequest { Email = "nonexistentuser@test.com" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(deleteRequest.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _accountController.Delete(deleteRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "existinguser@example.com",
                Password = "Password123!",
                Name = "Existing User"
            };

            var existingUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                Name = registerRequest.Name
            };

            // Mock the repository to return an existing user when GetUserByEmailAsync is called
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registerRequest.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _accountController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            var loginResult = Assert.IsType<LoginResult>(badRequestResult.Value);
            Assert.False(loginResult.Success);
            Assert.Equal("User already exists", loginResult.Message);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsSuccessfullyCreated()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                Name = "New User"
            };

            var newUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                Name = registerRequest.Name
            };

            // Mock the repository to return null when checking if user exists, meaning the user doesn't exist
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registerRequest.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Mock the repository to return a valid user when CreateUserAsync is called
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(registerRequest))
                .ReturnsAsync(newUser);

            // Mock the JWT generation secret key
            _configurationMock.Setup(config => config["JwtSettings:SecretKey"]).Returns("SuperSecretKeyThatHasAtLeast32Bytes");

            // Create the AccountController with the mocked dependencies
            var accountController = new AccountController(_userManagerMock.Object, _userRepositoryMock.Object, _configurationMock.Object);

            // Act
            var result = await accountController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var loginResult = Assert.IsType<LoginResult>(okResult.Value);
            Assert.True(loginResult.Success);
            Assert.Equal("User created", loginResult.Message);

            // Check that the token is not null or empty
            Assert.False(string.IsNullOrEmpty(loginResult.Token));

            // Optionally check if the token has three parts (header, payload, signature)
            Assert.Equal(3, loginResult.Token.Split('.').Length);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "WrongPassword"
            };

            var user = new ApplicationUser
            {
                Email = loginRequest.Email,
                UserName = loginRequest.Email
            };

            // Mock the user manager to return the user by email but fail password check
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password))
                .ReturnsAsync(false);  // Incorrect password

            // Create the AccountController
            var accountController = new AccountController(_userManagerMock.Object, _userRepositoryMock.Object, _configurationMock.Object);

            // Act
            var result = await accountController.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            var loginResult = Assert.IsType<LoginResult>(badRequestResult.Value);
            Assert.False(loginResult.Success);
            Assert.Equal("Invalid username or password", loginResult.Message);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "CorrectPassword"
            };

            var user = new ApplicationUser
            {
                Email = loginRequest.Email,
                UserName = loginRequest.Email
            };

            // Mock the user manager to find the user and check the password successfully
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password))
                .ReturnsAsync(true);  // Correct password

            // Mock the JWT generation secret key
            _configurationMock.Setup(config => config["JwtSettings:SecretKey"]).Returns("SuperSecretKeyThatHasAtLeast32Bytes");

            // Create the AccountController
            var accountController = new AccountController(_userManagerMock.Object, _userRepositoryMock.Object, _configurationMock.Object);

            // Act
            var result = await accountController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var loginResult = Assert.IsType<LoginResult>(okResult.Value);
            Assert.True(loginResult.Success);
            Assert.Equal("Login successful", loginResult.Message);

            // Check that the token is not null or empty
            Assert.False(string.IsNullOrEmpty(loginResult.Token));

            // Optionally check if the token has three parts (header, payload, signature)
            Assert.Equal(3, loginResult.Token.Split('.').Length);
        }
    }
}
