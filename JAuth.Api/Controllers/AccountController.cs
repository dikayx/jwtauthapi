using JAuth.Api.Data;
using JAuth.Api.Repositories;
using JAuth.UserEntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public AccountController(UserManager<ApplicationUser> userManager, IUserRepository repository, IConfiguration configuration)
        {
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration;
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
                return BadRequest(CreateLoginResult(false, "User already exists"));
            }

            var newUser = await _repository.CreateUserAsync(registerRequest);
            if (newUser == null)
            {
                return BadRequest(CreateLoginResult(false, "Failed to create user"));
            }

            var jwt = await GenerateJwt(newUser);
            return Ok(CreateLoginResult(true, "User created", jwt));
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
                return BadRequest(CreateLoginResult(false, "Invalid username or password"));
            }

            var jwt = await GenerateJwt(user);
            return Ok(CreateLoginResult(true, "Login successful", jwt));
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
                return BadRequest(CreateLoginResult(false, "User not found"));
            }

            var result = await _repository.UpdateUserAsync(updateRequest, user);
            if (result == null)
            {
                return BadRequest(CreateLoginResult(false, "Failed to update user"));
            }

            var jwt = await GenerateJwt(result);
            return Ok(CreateLoginResult(true, "User updated", jwt));
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
                return BadRequest(CreateLoginResult(false, "User not found"));
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
                return BadRequest(CreateLoginResult(false, "User not found"));
            }

            var result = await _repository.DeleteUserAsync(user);
            if (result == null)
            {
                return BadRequest(CreateLoginResult(false, "Failed to delete user"));
            }

            return Ok(CreateLoginResult(true, "User deleted"));
        }


        /// <summary>
        /// Asynchronously generates a JWT token for the given user.
        /// The token includes the user's name as a claim and expires after 1 hour.
        /// 
        /// Usage: var jwt = GenerateJwtToken(user).Result;
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>A JWT token as a string.</returns>
        private async Task<String?> GenerateJwt(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // TODO: Implement async operation for this method...?

            return tokenString;
        }

        /// Small helper method to create a LoginResult object.
        private LoginResult CreateLoginResult(bool success, string message, string? token = null)
        {
            return new LoginResult()
            {
                Success = success,
                Message = message,
                Token = token
            };
        }
    }
}
