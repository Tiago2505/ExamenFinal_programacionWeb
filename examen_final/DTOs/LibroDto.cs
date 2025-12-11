using System.ComponentModel.DataAnnotations;

namespace examen_final.DTOs
{

    public class LibroDto
    {
        [Required(ErrorMessage = "El título es requerido")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es requerido")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es requerida")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La editorial es requerida")]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de publicación es requerido")]
        [Range(0, 3000, ErrorMessage = "El año de publicación debe ser un número válido")]
        public int AñoPublicacion { get; set; }

        [Required(ErrorMessage = "El número de copias disponibles es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Copias disponibles debe ser un número válido")]
        public int CopiasDisponibles { get; set; }

        [Required(ErrorMessage = "El número total de copias es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Copias totales debe ser un número válido")]
        public int CopiasTotal { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida")]
        public string Ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de ingreso es requerida")]
        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    }
}