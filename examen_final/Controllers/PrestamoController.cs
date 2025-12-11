using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using examen_final.DTOs;
using examen_final.Services;

namespace examen_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamoController : ControllerBase
    {
        
       
            private readonly IPrestamoService _prestamoService;

            public PrestamoController(IPrestamoService prestamoService)
            {
                _prestamoService = prestamoService;
            }

            [HttpPost("{libroid}")]
            [Authorize] // Debe estar autenticado
            public async Task<IActionResult> CrearPrestamo(string libroid)
            {
                try
                {
                    // Obtener el usuario del token
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized("Usuario no autenticado");

                    var prestamo = await _prestamoService.CrearPrestamoAsync(userId, libroid);

                    return Ok(prestamo);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
}