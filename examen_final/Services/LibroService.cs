using examen_final.DTOs;
using examen_final.Models;

namespace examen_final.Services;

public class LibroService : ILibroService
{
    private readonly FirebaseServices _firebaseService;
    private readonly IConfiguration _configuration;

    public LibroService(FirebaseServices firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }
    
public async Task<LibroResponseDto> addLibro(LibroDto libroDto)
{
    try
    {
        // 1. Generar un ID único para el libro
        var libroId = Guid.NewGuid().ToString();

        // 2. Crear objeto Libro con los campos del DTO y el estado por defecto
        var libro = new Libro
        {
            isbn = libroId,
            titulo = libroDto.Titulo,
            autor = libroDto.Autor,
            categoria = libroDto.Categoria,
            editorial = libroDto.Editorial,
            añoPublicacion = libroDto.AñoPublicacion,
            copiasDisponibles = libroDto.CopiasDisponibles,
            copiasTotal = libroDto.CopiasTotal,
            ubicacion = libroDto.Ubicacion,
            estado = "disponible", // Valor por defecto
            descripcion = libroDto.Descripcion,
            fechaIngreso = libroDto.FechaIngreso
        };

        // 3. Obtener la colección "libros" de Firestore
        var librosCollection = _firebaseService.GetCollection("libros");

        // 4. Convertir a diccionario para guardar en Firestore
        var libroData = new Dictionary<string, object>
        {
            { "Id", libro.isbn },
            { "Titulo", libro.titulo },
            { "Autor", libro.autor },
            { "Categoria", libro.categoria },
            { "Editorial", libro.editorial },
            { "AñoPublicacion", libro.añoPublicacion },
            { "CopiasDisponibles", libro.copiasDisponibles },
            { "CopiasTotal", libro.copiasTotal },
            { "Ubicacion", libro.ubicacion },
            { "Estado", libro.estado },
            { "Descripcion", libro.descripcion },
            { "FechaIngreso", libro.fechaIngreso }
        };

        // 5. Guardar en Firestore
        await librosCollection.Document(libro.isbn).SetAsync(libroData);

        // 6. Retornar el libro agregado
        return new LibroResponseDto
        {
            Isbn = libro.isbn,
            Titulo = libro.titulo,
            Autor = libro.autor,
            Categoria = libro.categoria,
            Editorial = libro.editorial,
            AñoPublicacion = libro.añoPublicacion,
            CopiasDisponibles = libro.copiasDisponibles,
            CopiasTotal = libro.copiasTotal,
            Ubicacion = libro.ubicacion,
            Estado = libro.estado,
            Descripcion = libro.descripcion,
            FechaIngreso = libro.fechaIngreso
        };
    }
    catch (Exception ex)
    {
        throw new Exception($"Error al agregar libro: {ex.Message}");
    }
}

    
}