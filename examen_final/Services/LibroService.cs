using examen_final.DTOs;
using examen_final.Models;
using Google.Protobuf.WellKnownTypes;
using Google.Cloud.Firestore;

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

    public async Task<string> deleteLibro(string libroId)
    {
        try
        {
                    if (string.IsNullOrEmpty(libroId))
                        throw new Exception("El id no puede estar vacío.");

                    var bookDoc = _firebaseService.GetCollection("libros").Document(libroId);
                    var snapshot = await bookDoc.GetSnapshotAsync();

                    if (!snapshot.Exists)
                        throw new Exception("El libro no existe.");

                    var libro = snapshot.ConvertTo<Libro>();

                    // 1️⃣ No se puede eliminar si tiene préstamos activos
                    var prestamosActivos = await _firebaseService.GetCollection("prestamos")
                        .WhereEqualTo("Id", libroId)
                        .WhereEqualTo("Estado", "activo")
                        .GetSnapshotAsync();

                    if (prestamosActivos.Documents.Count > 0)
                        throw new Exception("No se puede eliminar un libro con préstamos activos.");

                    // 2️⃣ No se puede eliminar si tiene reservas pendientes/notificadas
                    var reservasPendientes = await _firebaseService.GetCollection("reservas")
                        .WhereEqualTo("Id", libroId)
                        .WhereIn("Estado", new[] { "pendiente", "notificada" })
                        .GetSnapshotAsync();

                    if (reservasPendientes.Documents.Count > 0)
                        throw new Exception("No se puede eliminar un libro con reservas pendientes.");


                    await bookDoc.DeleteAsync();

                    return "Libro eliminado";
        }
        catch (Exception e)
        {
            throw new Exception($"Error al eliminar el libro: {e.Message}");
        }
    }

   public async Task<LibroResponseDto?> UpdateLibroAsync(UpdateDto libroDto, string libroId)
{
    try
    {
        var libroDoc = _firebaseService.GetCollection("libros").Document(libroId);
        var snapshot = await libroDoc.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return null;
        }

        var updates = new Dictionary<string, object>();

        // Actualizar título
        if (!string.IsNullOrEmpty(libroDto.Titulo))
        {
            updates["Titulo"] = libroDto.Titulo;
        }

        // Actualizar autor
        if (!string.IsNullOrEmpty(libroDto.Autor))
        {
            updates["Autor"] = libroDto.Autor;
        }

        // Actualizar categoría
        if (!string.IsNullOrEmpty(libroDto.Categoria))
        {
            updates["Categoria"] = libroDto.Categoria;
        }

        // Actualizar editorial
        if (!string.IsNullOrEmpty(libroDto.Editorial))
        {
            updates["Editorial"] = libroDto.Editorial;
        }

        // Actualizar año de publicación
        if (libroDto.AñoPublicacion > 0 && libroDto.AñoPublicacion <= 3000)
        {
            updates["AñoPublicacion"] = libroDto.AñoPublicacion;
        }

        // Actualizar copias disponibles (no puede ser mayor a copiasTotal)
        if (libroDto.CopiasDisponibles >= 0 && libroDto.CopiasDisponibles <= libroDto.CopiasTotal)
        {
            updates["CopiasDisponibles"] = libroDto.CopiasDisponibles;
        }

        // Actualizar copias totales (debe ser mayor o igual a copiasDisponibles)
        if (libroDto.CopiasTotal >= libroDto.CopiasDisponibles)
        {
            updates["CopiasTotal"] = libroDto.CopiasTotal;
        }

        // Actualizar ubicación
        if (!string.IsNullOrEmpty(libroDto.Ubicacion))
        {
            updates["Ubicacion"] = libroDto.Ubicacion;
        }

        // Actualizar estado (solo valores válidos)
        if (!string.IsNullOrEmpty(libroDto.Estado) &&
            (libroDto.Estado.ToLower() == "disponible" ||
             libroDto.Estado.ToLower() == "agotado" ||
             libroDto.Estado.ToLower() == "en mantenimiento"))
        {
            updates["Estado"] = libroDto.Estado.ToLower();
        }

        // Actualizar descripción
        if (!string.IsNullOrEmpty(libroDto.Descripcion))
        {
            updates["Descripcion"] = libroDto.Descripcion;
        }

        // Actualizar fecha de ingreso
        if (libroDto.FechaIngreso != default)
        {
            updates["FechaIngreso"] = libroDto.FechaIngreso;
        }

        // Solo actualizar si hay campos válidos
        if (updates.Count > 0)
        {
            await libroDoc.UpdateAsync(updates);
        }

        // Retornar el documento actualizado
        var updatedSnapshot = await libroDoc.GetSnapshotAsync();
        var updated = updatedSnapshot.ToDictionary();

        return new LibroResponseDto
        {
            Titulo = updated["Titulo"]?.ToString() ?? "",
            Autor = updated["Autor"]?.ToString() ?? "",
            Isbn = updated["Id"]?.ToString() ?? "",
            Categoria = updated["Categoria"]?.ToString() ?? "",
            Editorial = updated["Editorial"]?.ToString() ?? "",
            AñoPublicacion = Convert.ToInt32(updated["AñoPublicacion"]),
            CopiasDisponibles = Convert.ToInt32(updated["CopiasDisponibles"]),
            CopiasTotal = Convert.ToInt32(updated["CopiasTotal"]),
            Ubicacion = updated["Ubicacion"]?.ToString() ?? "",
            Estado = updated["Estado"]?.ToString() ?? "",
            Descripcion = updated["Descripcion"]?.ToString() ?? "",
            FechaIngreso = ((Google.Cloud.Firestore.Timestamp)updated["FechaIngreso"]).ToDateTime()
        };
    }
    catch (Exception ex)
    {
        throw new Exception($"Error al actualizar libro: {ex.Message}");
    }
}

}