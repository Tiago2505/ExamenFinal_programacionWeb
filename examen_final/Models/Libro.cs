using Google.Cloud.Firestore;
using System;

namespace examen_final.Models
{
    [FirestoreData]
    public class Libro
    {
        [FirestoreProperty]
        public string Isbn { get; set; } = string.Empty; 
        
        [FirestoreProperty]
        public string titulo { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string autor { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string isbn { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string categoria { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string editorial { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public int añoPublicacion { get; set; } = 0;
        
        [FirestoreProperty]
        public int copiasDisponibles { get; set; } = 0;
        
        [FirestoreProperty]
        public int copiasTotal { get; set; } = 0;
        
        [FirestoreProperty]
        public string ubicacion { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string estado { get; set; } = "disponible"; // "disponible", "agotado" o "en mantenimiento"
        
        [FirestoreProperty]
        public string descripcion { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public DateTime fechaIngreso { get; set; } = DateTime.UtcNow;
    }
}