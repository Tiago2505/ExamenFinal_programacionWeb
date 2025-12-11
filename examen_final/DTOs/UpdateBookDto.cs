namespace examen_final.DTOs;

public class UpdateBookDto
{
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Categoria { get; set; }
    public string? Editorial { get; set; }
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public string? Estado { get; set; }
    public int? CopiasTotal { get; set; }
    public int? CopiasDisponibles { get; set; }
}