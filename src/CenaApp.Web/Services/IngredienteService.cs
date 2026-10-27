// Administración del catálogo de ingredientes. Como los alérgenos de las recetas se calculan desde los
// ingredientes, corregir un ingrediente corrige al instante todas las recetas que lo usan (VN-04, VN-23).
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using CenaApp.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Services;

public class IngredienteService : IIngredienteService
{
    private readonly AppDbContext _db;

    public IngredienteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<IngredienteFilaViewModel>> ListarAsync(string? buscar = null)
    {
        var filas = await _db.Ingredientes
            .OrderBy(i => i.Nombre)
            .Select(i => new IngredienteFilaViewModel
            {
                Id = i.Id,
                Nombre = i.Nombre,
                Grupo = i.Grupo,
                Sinonimos = i.Sinonimos,
                Alergenos = i.Alergenos.Select(a => a.Nombre).ToList(),
                RecetasQueLoUsan = _db.RecetaIngredientes.Count(ri => ri.IngredienteId == i.Id),
            })
            .ToListAsync();

        if (string.IsNullOrWhiteSpace(buscar))
        {
            return filas;
        }
        // Mismo criterio que la despensa: sin mayúsculas ni tildes, en nombre o sinónimos.
        var texto = DespensaService.Normalizar(buscar);
        return filas
            .Where(f => DespensaService.Normalizar(f.Nombre).Contains(texto)
                || DespensaService.Normalizar(f.Sinonimos ?? "").Contains(texto))
            .ToList();
    }

    public async Task<Ingrediente?> ObtenerAsync(int id)
    {
        return await _db.Ingredientes
            .Include(i => i.Alergenos)
            .Include(i => i.RestriccionesIncompatibles)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CargarOpcionesAsync(IngredienteFormularioViewModel formulario)
    {
        formulario.Alergenos = await _db.Alergenos.OrderBy(a => a.Nombre).ToListAsync();
        formulario.Restricciones = await _db.Restricciones.OrderBy(r => r.Nombre).ToListAsync();
        formulario.Grupos = await _db.Ingredientes.Select(i => i.Grupo).Distinct().OrderBy(g => g).ToListAsync();
    }

    public async Task<Dictionary<string, string>> ValidarAsync(IngredienteFormularioViewModel formulario)
    {
        var errores = new Dictionary<string, string>();
        var sinonimos = Separar(formulario.Sinonimos);

        // VAL-24: cada sinónimo de hasta 30 caracteres.
        if (sinonimos.Any(s => s.Length > 30))
        {
            errores[nameof(formulario.Sinonimos)] = "Cada sinónimo puede tener hasta 30 caracteres.";
        }

        // VAL-22: ni el nombre ni los sinónimos pueden coincidir con el nombre o un sinónimo de OTRO
        // ingrediente (sin importar mayúsculas ni tildes). Si no, la búsqueda de la despensa sería ambigua.
        var otros = await _db.Ingredientes.Where(i => i.Id != formulario.Id).ToListAsync();
        foreach (var palabra in sinonimos.Prepend(formulario.Nombre))
        {
            var repetido = DespensaService.Buscar(otros, palabra);
            if (repetido is not null)
            {
                var campo = palabra == formulario.Nombre ? nameof(formulario.Nombre) : nameof(formulario.Sinonimos);
                errores[campo] = $"\"{palabra.Trim()}\" ya se usa en el ingrediente {repetido.Nombre}.";
                break;
            }
        }

        return errores;
    }

    public async Task<Ingrediente> CrearAsync(IngredienteFormularioViewModel formulario)
    {
        var ingrediente = new Ingrediente();
        await CopiarAsync(formulario, ingrediente);
        _db.Ingredientes.Add(ingrediente);
        await _db.SaveChangesAsync();
        return ingrediente;
    }

    public async Task<bool> ActualizarAsync(int id, IngredienteFormularioViewModel formulario)
    {
        var ingrediente = await ObtenerAsync(id);
        if (ingrediente is null)
        {
            return false;
        }
        await CopiarAsync(formulario, ingrediente);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> EliminarAsync(int id)
    {
        var ingrediente = await _db.Ingredientes.FindAsync(id);
        if (ingrediente is null)
        {
            return $"No existe un ingrediente con id {id}.";
        }

        // VN-21: no se borra un ingrediente que usa alguna receta (la base también lo impide: DeleteBehavior.Restrict).
        var recetas = await _db.RecetaIngredientes.CountAsync(ri => ri.IngredienteId == id);
        if (recetas > 0)
        {
            return $"No se puede eliminar {ingrediente.Nombre}: se usa en {recetas} {(recetas == 1 ? "receta" : "recetas")}.";
        }

        _db.Ingredientes.Remove(ingrediente);
        await _db.SaveChangesAsync();
        return null;
    }

    private async Task CopiarAsync(IngredienteFormularioViewModel formulario, Ingrediente ingrediente)
    {
        ingrediente.Nombre = formulario.Nombre.Trim();
        ingrediente.Grupo = formulario.Grupo.Trim();
        var sinonimos = Separar(formulario.Sinonimos);
        ingrediente.Sinonimos = sinonimos.Count == 0 ? null : string.Join(", ", sinonimos);
        ingrediente.CuentaEnDespensa = formulario.CuentaEnDespensa;
        ingrediente.Alergenos = await _db.Alergenos.Where(a => formulario.AlergenosIds.Contains(a.Id)).ToListAsync();
        ingrediente.RestriccionesIncompatibles = await _db.Restricciones
            .Where(r => formulario.RestriccionesIncompatiblesIds.Contains(r.Id)).ToListAsync();
    }

    // "palta,  aguacate ,," → ["palta", "aguacate"]
    private static List<string> Separar(string? sinonimos) =>
        (sinonimos ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
