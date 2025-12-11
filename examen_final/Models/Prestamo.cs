using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace examen_final.Models
{
    [FirestoreData]
    public class Prestamo
    {
        [FirestoreProperty]
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public string usuarioId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required(ErrorMessage = "El ID del libro es requerido")]
        public string libroId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required(ErrorMessage = "La fecha de préstamo es requerida")]
        public DateTime fechaPrestamo { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        [Required(ErrorMessage = "La fecha de devolución esperada es requerida")]
        public DateTime fechaDevolucionEsperada { get; set; }

        [FirestoreProperty]
        public DateTime? fechaDevolucionReal { get; set; } = null;

        [FirestoreProperty]
        public int diasRetraso { get; set; } = 0; // Calculado automáticamente según la diferencia entre fechas

        [FirestoreProperty]
        public double multaGenerada { get; set; } = 0; // 50 Lempiras por día de retraso

        [FirestoreProperty]
        [Required(ErrorMessage = "El estado del préstamo es requerido")]
        [RegularExpression("activo|devuelto|vencido", ErrorMessage = "El estado debe ser 'activo', 'devuelto' o 'vencido'")]
        public string estado { get; set; } = "activo"; // Valor por defecto

        [FirestoreProperty]
        [Range(0, int.MaxValue, ErrorMessage = "El número de renovaciones debe ser un valor válido")]
        public int renovaciones { get; set; } = 0; // Contador de renovaciones
    }
}