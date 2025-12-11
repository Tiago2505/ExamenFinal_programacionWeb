using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using examen_final.DTOs;
using examen_final.Services;

namespace examen_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
        
        [HttpGet("me")]
        [Authorize] // Requiere que se autentique si o si
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                //Obtener el ID del usuario del token jwt

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new {error = "Token no valido"});
                }

                var user = await _authService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(new {error = "Usuario no encontrado"});
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
        
    }
}