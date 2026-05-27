using GymSupport.Repository.Models.DTOs.Auth;
using GymSupport.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSupport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
        {
            try
            {
                var id = await _authService.RegisterCustomerAsync(request);
                return CreatedAtAction(nameof(RegisterCustomer), new { id }, new { id });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("register/manager")]
        public async Task<IActionResult> RegisterManager([FromBody] RegisterManagerRequest request)
        {
            try
            {
                var id = await _authService.RegisterManagerAsync(request);
                return CreatedAtAction(nameof(RegisterManager), new { id }, new { id });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var resp = await _authService.LoginAsync(request);
                return Ok(resp);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return Ok(new { userId });
        }
    }
}