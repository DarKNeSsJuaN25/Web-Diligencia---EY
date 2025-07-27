using Microsoft.AspNetCore.Mvc;
using DiligenciaProveedores.Application.Dtos.Auth;
using DiligenciaProveedores.Application.Interfaces; 
namespace DiligenciaProveedores.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterUserAsync(request);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Creación de usuario fallida!", Errors = errors });
            }
            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", Message = "Usuario creado exitosamente!" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginUserAsync(request);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAdminAsync(request);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Creación de usuario admin fallida!", Errors = errors });
            }
            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", Message = "Usuario Admin creado exitosamente!" });
        }

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] VerifyTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }

            var isValid = await _authService.VerifyTokenAsync(request.Token);

            if (isValid)
            {
                return Ok(new { IsValid = true });
            }
            return Unauthorized(new { IsValid = false, Message = "Invalid or expired token." });
        }
    }
}