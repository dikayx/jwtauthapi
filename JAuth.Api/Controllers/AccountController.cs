using JAuth.Api.Data;
using JAuth.Api.Factories;
using JAuth.Api.Repositories;
using JAuth.Api.Security;
using JAuth.UserEntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JAuth.Api.Controllers
{
    // base address: api/account
    [Route(Constants.ApiRoute)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<ApplicationUser> userManager, IUserRepository repository, IConfiguration configuration)
        {
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration;
            _tokenService = new TokenService(_configuration);
        }

        // POST: api/Account/Register
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var existingUser = await _repository.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return BadRequest(LoginResultFactory.Create(false, "User already exists"));
            }

            var newUser = await _repository.CreateUserAsync(registerRequest);
            if (newUser == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "Failed to create user"));
            }

            var jwt = await _tokenService.GenerateJwtAsync(newUser);
            return Ok(LoginResultFactory.Create(true, "User created", jwt));
        }

        // POST: api/Account/Login
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return BadRequest(LoginResultFactory.Create(false, "Invalid username or password"));
            }

            var jwt = await _tokenService.GenerateJwtAsync(user);
            return Ok(LoginResultFactory.Create(true, "Login successful", jwt));
        }

        // PUT: api/Account/Update
        [HttpPut("update")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateAccountRequest updateRequest)
        {
            var user = await _userManager.FindByEmailAsync(updateRequest.Email);
            if (user == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "User not found"));
            }

            var result = await _repository.UpdateUserAsync(updateRequest, user);
            if (result == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "Failed to update user"));
            }

            var jwt = await _tokenService.GenerateJwtAsync(result);
            return Ok(LoginResultFactory.Create(true, "User updated", jwt));
        }

        // GET: api/Account/Get/{email}
        [HttpGet("get/{email}")]
        [ProducesResponseType(200, Type = typeof(ApplicationUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _repository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "User not found"));
            }

            return Ok(user);
        }

        // DELETE: api/Account/Delete
        [HttpDelete("delete")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteAccountRequest deleteRequest)
        {
            var user = await _userManager.FindByEmailAsync(deleteRequest.Email);
            if (user == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "User not found"));
            }

            var result = await _repository.DeleteUserAsync(user);
            if (result == null)
            {
                return BadRequest(LoginResultFactory.Create(false, "Failed to delete user"));
            }

            return Ok(LoginResultFactory.Create(true, "User deleted"));
        }
    }
}
