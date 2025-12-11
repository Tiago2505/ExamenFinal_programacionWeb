using System.ComponentModel.DataAnnotations;

namespace examen_final.DTOs
{
    public class UpdateDto
    {
        public string? Titulo { get; set; }

        public string? Autor { get; set; }

        public string? Categoria { get; set; }

        public string? Editorial { get; set; }

        [Range(0, 3000, ErrorMessage = "El año de publicación debe ser un número válido")]
        public int? AñoPublicacion { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Copias disponibles debe ser un número válido")]
        public int? CopiasDisponibles { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Copias totales debe ser un número válido")]
        public int? CopiasTotal { get; set; }

        public string? Ubicacion { get; set; }

        [RegularExpression("disponible|agotado|en mantenimiento", ErrorMessage = "El estado debe ser 'disponible', 'agotado' o 'en mantenimiento'")]
        public string? Estado { get; set; }

        public string? Descripcion { get; set; }

        public DateTime? FechaIngreso { get; set; }
    }
}