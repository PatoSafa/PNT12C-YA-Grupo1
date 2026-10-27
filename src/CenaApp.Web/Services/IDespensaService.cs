// Contrato del servicio de despensa: los ingredientes que el usuario tiene en casa y qué puede cocinar con ellos.
using CenaApp.Web.Models;

namespace CenaApp.Web.Services;

public interface IDespensaService
{
    // Ingredientes de la despensa del usuario, por nombre.
    Task<List<Ingrediente>> ObtenerAsync(int usuarioId);

    // Todo el catálogo de ingredientes (para las sugerencias del buscador).
    Task<List<Ingrediente>> CatalogoAsync();

    // Agrega un ingrediente buscándolo por nombre o sinónimo. Devuelve un mensaje de error si no existe
    // en el catálogo (VAL-13) o si ya estaba (VAL-12); null si se agregó.
    Task<string?> AgregarAsync(int usuarioId, string texto);

    Task QuitarAsync(int usuarioId, int ingredienteId);

    Task VaciarAsync(int usuarioId);

    // "Podés cocinar" (100 %) y "Casi listas" (50–99 %), respetando las alergias y restricciones del usuario.
    Task<ResultadoDespensa> SugerenciasAsync(int usuarioId);
}

// Una receta analizada contra la despensa: qué porcentaje de sus ingredientes hay y qué falta comprar.
public record AnalisisReceta(Receta Receta, int Porcentaje, List<RecetaIngrediente> Faltantes);

public record ResultadoDespensa(List<AnalisisReceta> PodesCocinar, List<AnalisisReceta> CasiListas);
