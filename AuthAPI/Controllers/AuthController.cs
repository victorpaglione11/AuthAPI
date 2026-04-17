using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("token_policy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO dto)
        {
            await _service.RegisterAsync(dto.Username, dto.Password);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO dto)
        {
            var token = await _service.LoginAsync(dto.Username, dto.Password);
            return Ok(new { token });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDTO dto)
        {
            var result = await _service.RefreshAsync(dto.RefreshToken);
            return Ok(result);
        }
    }
}
