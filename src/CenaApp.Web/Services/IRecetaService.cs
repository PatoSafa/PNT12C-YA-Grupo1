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
}
