using System.ComponentModel.DataAnnotations;

namespace examen_final.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email es inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El password es requerido")]
        [MinLength(6, ErrorMessage = "El password debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es requerida")]
        [Range(0, 100, ErrorMessage = "La edad debe ser un número válido")]
        public int edad { get; set; }

        [Required(ErrorMessage = "El número de identidad es requerido")]
        public string numeroidentidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string telefono { get; set; } = string.Empty;

    }
}