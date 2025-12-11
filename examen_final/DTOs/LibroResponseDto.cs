namespace examen_final.DTOs
{
    public class LibroResponseDto
    {
        public string Titulo { get; set; } = string.Empty;
        
        public string Autor { get; set; } = string.Empty;
        
        public string Isbn { get; set; } = string.Empty;
        
        public string Categoria { get; set; } = string.Empty;
        
        public string Editorial { get; set; } = string.Empty;
        
        public int AñoPublicacion { get; set; } = 0;
        
        public int CopiasDisponibles { get; set; } = 0;
        
        public int CopiasTotal { get; set; } = 0;
        
        public string Ubicacion { get; set; } = string.Empty;
        
        public string Estado { get; set; } = "disponible"; // valor por defecto
        
        public string Descripcion { get; set; } = string.Empty;
        
        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    }
}