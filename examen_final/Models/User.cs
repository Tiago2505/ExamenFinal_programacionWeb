using Google.Cloud.Firestore;

namespace examen_final.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string nombre { get; set; } = string.Empty;

        [FirestoreProperty]
        public string apellido { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public int edad {get; set;} = 0;
        [FirestoreProperty]
        public string numeroidentidad { get; set; } = string.Empty;
        [FirestoreProperty]
        public string telefono { get;set; } = string.Empty;
        [FirestoreProperty]
        public string Role { get; set; } = "user"; //"admin" o "user"
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [FirestoreProperty]
        public bool IsActive { get; set; } = true;
        
        [FirestoreProperty]
        public double multa { get; set; } = 0;
    }
}