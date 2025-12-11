using System.ComponentModel.DataAnnotations;

namespace examen_final.DTOs;

public class LoginDto
{
    [Required(ErrorMessage ="El email es requerido")]
    [EmailAddress(ErrorMessage ="El email es invalido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El password es requerido")]
    public string Password { get; set; } = string.Empty;
}