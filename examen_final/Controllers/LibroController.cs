using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using examen_final.DTOs;
using examen_final.Services;

namespace examen_final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController : ControllerBase
    {
        private readonly ILibroService _libroService;
        public LibroController(ILibroService libroService)
        {
            _libroService = libroService;
        }
        
        [HttpPost]
        [Authorize(Roles = "admin,bibliotecario")]
        public async Task<IActionResult> addLibro([FromBody] LibroDto libro)
        {
            try
            {
                var response = await _libroService.addLibro(libro);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
        [HttpDelete("{libroId}")]
        [Authorize(Roles = "admin,bibliotecario")]
        public async Task<IActionResult> deleteLibro(string libroId)
        {
            try
            {
                var response = await _libroService.deleteLibro(libroId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
    }
    
}