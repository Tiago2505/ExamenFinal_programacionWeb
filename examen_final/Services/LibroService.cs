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

    public async Task<Libro?> UpdateLibro(UpdateBookDto dto, string libroId)
    {
        try
        {
            if (string.IsNullOrEmpty(libroId))
                throw new Exception("El id no puede estar vacío.");

            var bookDoc = _firebaseService.GetCollection("libros").Document(libroId);
            var snapshot = await bookDoc.GetSnapshotAsync();

            if (!snapshot.Exists)
                throw new Exception("El libro no existe.");

            var libroActual = snapshot.ConvertTo<Libro>();

            int copiasPrestadas = libroActual.copiasTotal - libroActual.copiasDisponibles;

            // Validar copiasTotal si se intenta modificar
            if (dto.CopiasTotal.HasValue)
            {
                if (dto.CopiasTotal < copiasPrestadas)
                    throw new Exception("No se puede reducir 'copiasTotal' por debajo de las copias prestadas.");

                if (dto.CopiasTotal.Value < 0)
                    throw new Exception("copiasTotal no puede ser negativa.");
            }

            // Validar copiasDisponibles si se intenta modificar
            if (dto.CopiasDisponibles.HasValue)
            {
                int total = dto.CopiasTotal ?? libroActual.copiasTotal;

                if (dto.CopiasDisponibles > total)
                    throw new Exception("'copiasDisponibles' no puede ser mayor que 'copiasTotal'.");
            }


            var updates = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(dto.Titulo)) updates["Titulo"] = dto.Titulo;
            if (!string.IsNullOrEmpty(dto.Autor)) updates["Autor"] = dto.Autor;
            if (!string.IsNullOrEmpty(dto.Categoria)) updates["Categoria"] = dto.Categoria;
            if (!string.IsNullOrEmpty(dto.Editorial)) updates["Editorial"] = dto.Editorial;
            if (!string.IsNullOrEmpty(dto.Descripcion)) updates["Descripcion"] = dto.Descripcion;
            if (!string.IsNullOrEmpty(dto.Ubicacion)) updates["Ubicacion"] = dto.Ubicacion;
            if (!string.IsNullOrEmpty(dto.Estado)) updates["Estado"] = dto.Estado;

            if (dto.CopiasTotal.HasValue) updates["CopiasTotal"] = dto.CopiasTotal.Value;
            if (dto.CopiasDisponibles.HasValue) updates["CopiasDisponibles"] = dto.CopiasDisponibles.Value;

            // Si no hay nada que actualizar
            if (updates.Count == 0)
                throw new Exception("No hay campos para actualizar.");
            await bookDoc.UpdateAsync(updates);

            // Obtener libro actualizado
            var updatedSnap = await bookDoc.GetSnapshotAsync();
            return updatedSnap.ConvertTo<Libro>();
        }
        catch (Exception e)
        {
            throw new Exception($"Error al actualizar el libro: {e.Message}");
        }
    }
}