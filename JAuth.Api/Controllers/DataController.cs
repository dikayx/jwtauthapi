using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JAuth.Api.Controllers
{
    // base address: api/data
    [Route(Constants.ApiRoute)]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            _logger.LogInformation($"/public endpoint accessed @ {DateTime.Now}");
            return Ok("This is a public endpoint. No authentication required.");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            _logger.LogInformation($"/protected endpoint accessed @ {DateTime.Now}");
            return Ok("This is a protected endpoint. You are authenticated!");
        }
    }
}
