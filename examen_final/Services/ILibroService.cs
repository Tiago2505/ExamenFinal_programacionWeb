using examen_final.DTOs;
using examen_final.Models;

namespace examen_final.Services
{
    public interface ILibroService
    {
        Task<LibroResponseDto> addLibro(LibroDto libro);
        Task<string> deleteLibro(string libroId);
        Task<LibroResponseDto> UpdateLibroAsync(UpdateDto libroDto,string  libroId);
    }
}