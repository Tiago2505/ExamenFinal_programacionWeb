namespace examen_final.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using examen_final.DTOs;
using examen_final.Models;


public class PrestamoService : IPrestamoService
{
    private readonly FirebaseServices _firebaseService;
    private readonly IConfiguration _configuration;

    public PrestamoService(FirebaseServices firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }
    
    public async Task<Prestamo> CrearPrestamoAsync(string usuarioId, string libroId)
    {
        // 1. Obtener usuario
        var userCollection = _firebaseService.GetCollection("users");
        var userDoc = await userCollection.Document(usuarioId).GetSnapshotAsync();
        if (!userDoc.Exists)
            throw new Exception("Usuario no encontrado");

        var user = userDoc.ConvertTo<User>();

        // 2. Verificar multas pendientes
        if (user.multa > 500)
            throw new Exception("El usuario tiene multas pendientes mayores a 500 Lempiras");

        // 3. Contar préstamos activos del usuario
        var prestamosCollection = _firebaseService.GetCollection("prestamos");
        var activosQuery = prestamosCollection
            .WhereEqualTo("usuarioId", usuarioId)
            .WhereEqualTo("estado", "activo");
        var activosSnapshot = await activosQuery.GetSnapshotAsync();

        if (activosSnapshot.Count >= 3)
            throw new Exception("El usuario ya tiene 3 préstamos activos");

        // 4. Obtener libro
        var libroCollection = _firebaseService.GetCollection("libros");
        var libroDoc = await libroCollection.Document(libroId).GetSnapshotAsync();
        if (!libroDoc.Exists)
            throw new Exception("Libro no encontrado");

        var libro = libroDoc.ConvertTo<Libro>();
        
        var libroData = libroDoc.ToDictionary();
        int copiasDisponibles = Convert.ToInt32(libroData["CopiasDisponibles"]);

        // 5. Verificar copias disponibles
        if (copiasDisponibles <= 0)
            throw new Exception("No hay copias disponibles de este libro");

        // 6. Decrementar copias disponibles del libro
        libro.copiasDisponibles -= 1;
        await libroCollection.Document(libroId).UpdateAsync(new Dictionary<string, object>
        {
            { "copiasDisponibles", libro.copiasDisponibles }
        });

        // 7. Crear el préstamo
        var prestamo = new Prestamo
        {
            usuarioId = usuarioId,
            libroId = libroId,
            fechaPrestamo = DateTime.UtcNow,
            fechaDevolucionEsperada = DateTime.UtcNow.AddDays(14),
            estado = "activo",
            diasRetraso = 0,
            multaGenerada = 0,
            renovaciones = 0
        };

        await prestamosCollection.AddAsync(prestamo);

        return prestamo;
    }
}
