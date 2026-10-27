// Contrato del servicio de recetas: lo que los controllers pueden pedirle.
using CenaApp.Web.Models;
using CenaApp.Web.ViewModels;

namespace CenaApp.Web.Services;

public interface IRecetaService
{
    // Todas las recetas; si viene "nombre", solo las que lo contienen (sin distinguir mayúsculas).
    Task<List<Receta>> ListarAsync(string? nombre = null);

    // Una receta con su categoría, ingredientes (y sus alérgenos), pasos y gustos. null si no existe.
    Task<Receta?> ObtenerAsync(int id);

    // Controla las reglas que las Data Annotations no pueden ver (nombre repetido, categoría o
    // ingredientes inexistentes, ingredientes repetidos). Devuelve los errores por campo; vacío = todo bien.
    Task<Dictionary<string, string>> ValidarAsync(RecetaCrearDto datos);

    Task<Receta> CrearAsync(RecetaCrearDto datos);

    // false si la receta no existe.
    Task<bool> EliminarAsync(int id);

    // Recetas para el Inicio de un usuario: sin sus alérgenos, aptas para sus restricciones y
    // ordenadas por sus gustos. Opcional: texto a buscar en nombre o descripción, y un gusto para filtrar.
    Task<List<Receta>> ListarParaUsuarioAsync(int usuarioId, string? texto = null, int? gustoId = null);

    // Marca o desmarca la receta como favorita. Devuelve el estado nuevo (true = quedó favorita),
    // o null si la receta no existe.
    Task<bool?> AlternarFavoritoAsync(int usuarioId, int recetaId);

    Task<HashSet<int>> IdsFavoritosAsync(int usuarioId);

    Task<List<Receta>> ListarFavoritosAsync(int usuarioId);
}
