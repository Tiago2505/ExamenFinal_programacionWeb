namespace examen_final.Services;
using examen_final.DTOs;
using examen_final.Models;

public interface IPrestamoService
{
    Task<Prestamo> CrearPrestamoAsync(string usuarioId, string libroId);
}