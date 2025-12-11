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
        [Authorize(Roles = "admin")]
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
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,bibliotecario")]
        public async Task<IActionResult> UpdateBook(string id, [FromBody] UpdateBookDto dto)
        {
            try
            {
                var updated = await _libroService.UpdateLibro(dto, id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        
    }
    
}