// Reglas de negocio de las recetas: consultas, alta, baja y cálculo de alérgenos desde los ingredientes.
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using CenaApp.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Services;

public class RecetaService : IRecetaService
{
    private readonly AppDbContext _db;

    public RecetaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Receta>> ListarAsync(string? nombre = null)
    {
        var consulta = _db.Recetas.Include(r => r.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var buscado = nombre.Trim().ToLower();
            consulta = consulta.Where(r => r.Nombre.ToLower().Contains(buscado));
        }

        return await consulta.OrderBy(r => r.Nombre).ToListAsync();
    }

    public async Task<Receta?> ObtenerAsync(int id)
    {
        return await _db.Recetas
            .Include(r => r.Categoria)
            .Include(r => r.Gustos)
            .Include(r => r.Pasos)
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.Alergenos)
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.RestriccionesIncompatibles)
            .AsSplitQuery() // varias consultas chicas en vez de un JOIN enorme con filas repetidas
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Dictionary<string, string>> ValidarAsync(RecetaCrearDto datos)
    {
        var errores = new Dictionary<string, string>();

        // VAL-15: nombre único (sin distinguir mayúsculas ni espacios de los costados).
        var nombre = datos.Nombre.Trim().ToLower();
        if (await _db.Recetas.AnyAsync(r => r.Nombre.ToLower() == nombre))
        {
            errores[nameof(datos.Nombre)] = "Ya existe una receta con ese nombre.";
        }

        if (!await _db.Categorias.AnyAsync(c => c.Id == datos.CategoriaId))
        {
            errores[nameof(datos.CategoriaId)] = $"No existe la categoría {datos.CategoriaId}.";
        }

        // VAL-18: sin ingredientes repetidos y todos del catálogo.
        var ids = datos.Ingredientes.Select(i => i.IngredienteId).ToList();
        if (ids.Count != ids.Distinct().Count())
        {
            errores[nameof(datos.Ingredientes)] = "Hay ingredientes repetidos.";
        }
        else
        {
            var existentes = await _db.Ingredientes.CountAsync(i => ids.Contains(i.Id));
            if (existentes != ids.Count)
            {
                errores[nameof(datos.Ingredientes)] = "Algún ingrediente no existe en el catálogo.";
            }
        }

        // VAL-19: cada paso con texto y hasta 500 caracteres.
        if (datos.Pasos.Any(p => string.IsNullOrWhiteSpace(p) || p.Length > 500))
        {
            errores[nameof(datos.Pasos)] = "Cada paso tiene que tener texto y hasta 500 caracteres.";
        }

        return errores;
    }

    public async Task<Receta> CrearAsync(RecetaCrearDto datos)
    {
        var receta = new Receta
        {
            Nombre = datos.Nombre.Trim(),
            Descripcion = datos.Descripcion?.Trim(),
            Emoji = string.IsNullOrWhiteSpace(datos.Emoji) ? "🍽️" : datos.Emoji.Trim(),
            TiempoMin = datos.TiempoMin,
            Porciones = datos.Porciones,
            Calorias = datos.Calorias,
            Dificultad = datos.Dificultad!.Value,
            CategoriaId = datos.CategoriaId,
            Ingredientes = datos.Ingredientes.Select(i => new RecetaIngrediente
            {
                IngredienteId = i.IngredienteId,
                Cantidad = i.Cantidad,
                Unidad = i.Unidad,
            }).ToList(),
            Pasos = datos.Pasos.Select((texto, i) => new Paso { Orden = i + 1, Texto = texto.Trim() }).ToList(),
        };

        _db.Recetas.Add(receta);
        await _db.SaveChangesAsync();
        return receta;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var receta = await _db.Recetas.FindAsync(id);
        if (receta is null)
        {
            return false;
        }

        // Pasos, ingredientes de la receta y favoritos se borran en cascada (VN-22).
        _db.Recetas.Remove(receta);
        await _db.SaveChangesAsync();
        return true;
    }

    // Alérgenos de una receta = todos los alérgenos de sus ingredientes, sin repetir (VN-04).
    // Necesita la receta cargada con Ingredientes → Ingrediente → Alergenos.
    public static List<string> CalcularAlergenos(Receta receta)
    {
        return receta.Ingredientes
            .SelectMany(ri => ri.Ingrediente.Alergenos)
            .Select(a => a.Nombre)
            .Distinct()
            .Order()
            .ToList();
    }
}
