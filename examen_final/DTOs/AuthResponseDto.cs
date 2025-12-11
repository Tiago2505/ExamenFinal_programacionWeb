namespace examen_final.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public int Edad { get; set; } = 0;

        public string NumeroIdentidad { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public double Multa { get; set; } = 0;
    }
}