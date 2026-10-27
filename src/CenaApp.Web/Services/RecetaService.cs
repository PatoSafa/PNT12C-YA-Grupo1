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

    public async Task<List<Receta>> ListarParaUsuarioAsync(int usuarioId, string? texto = null, int? gustoId = null)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Restricciones)
            .Include(u => u.Alergenos)
            .Include(u => u.Gustos)
            .AsSplitQuery()
            .FirstAsync(u => u.Id == usuarioId);

        var consulta = ConIngredientesYGustos(_db.Recetas);

        // RF11: búsqueda por texto en nombre o descripción.
        if (!string.IsNullOrWhiteSpace(texto))
        {
            var buscado = texto.Trim().ToLower();
            consulta = consulta.Where(r => r.Nombre.ToLower().Contains(buscado)
                || (r.Descripcion != null && r.Descripcion.ToLower().Contains(buscado)));
        }

        // RF11: filtro por un gusto.
        if (gustoId is not null)
        {
            consulta = consulta.Where(r => r.Gustos.Any(g => g.Id == gustoId));
        }

        // El filtro de aptitud se hace en memoria con las funciones de abajo (son las que tienen tests).
        var recetas = await consulta.ToListAsync();
        return FiltrarYOrdenar(recetas,
            usuario.Restricciones.Select(r => r.Id).ToList(),
            usuario.Alergenos.Select(a => a.Id).ToList(),
            usuario.Gustos.Select(g => g.Id).ToList());
    }

    // ---------- Favoritos (RF12) ----------

    public async Task<bool?> AlternarFavoritoAsync(int usuarioId, int recetaId)
    {
        var receta = await _db.Recetas.FindAsync(recetaId);
        if (receta is null)
        {
            return null;
        }

        var usuario = await _db.Usuarios.Include(u => u.Favoritos).FirstAsync(u => u.Id == usuarioId);
        var yaEstaba = usuario.Favoritos.Any(f => f.Id == recetaId);
        if (yaEstaba)
        {
            usuario.Favoritos.RemoveAll(f => f.Id == recetaId);
        }
        else
        {
            usuario.Favoritos.Add(receta);
        }

        // EF Core agrega o borra la fila (UsuarioId, RecetaId) de la tabla Favoritos.
        await _db.SaveChangesAsync();
        return !yaEstaba;
    }

    public async Task<HashSet<int>> IdsFavoritosAsync(int usuarioId)
    {
        var ids = await _db.Usuarios
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.Favoritos.Select(f => f.Id))
            .ToListAsync();
        return ids.ToHashSet();
    }

    public async Task<List<Receta>> ListarFavoritosAsync(int usuarioId)
    {
        return await _db.Usuarios
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.Favoritos)
            .OrderBy(r => r.Nombre)
            .ToListAsync();
    }

    // Include de todo lo que necesitan los cálculos de aptitud.
    private static IQueryable<Receta> ConIngredientesYGustos(IQueryable<Receta> recetas) =>
        recetas
            .Include(r => r.Categoria)
            .Include(r => r.Gustos)
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.Alergenos)
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.RestriccionesIncompatibles)
            .AsSplitQuery();

    // ---------------------------------------------------------------------------------------------
    // Reglas de aptitud (VN-04 a VN-08). Son funciones puras: reciben las recetas ya cargadas con sus
    // ingredientes y devuelven un resultado, sin tocar la base. Por eso se pueden testear (CenaApp.Tests).
    // ---------------------------------------------------------------------------------------------

    // Alérgenos de una receta = todos los alérgenos de sus ingredientes, sin repetir (VN-04).
    public static List<string> CalcularAlergenos(Receta receta)
    {
        return receta.Ingredientes
            .SelectMany(ri => ri.Ingrediente.Alergenos)
            .Select(a => a.Nombre)
            .Distinct()
            .Order()
            .ToList();
    }

    // Una receta es apta para una restricción si NINGUNO de sus ingredientes es incompatible con ella.
    // Con varias restricciones tiene que ser apta para todas (filtro estricto, VN-06).
    public static bool EsApta(Receta receta, ICollection<int> restriccionesIds)
    {
        return !receta.Ingredientes.Any(ri =>
            ri.Ingrediente.RestriccionesIncompatibles.Any(r => restriccionesIds.Contains(r.Id)));
    }

    // De un catálogo de restricciones, las que la receta cumple (para mostrar "Apta para: ...").
    public static List<string> RestriccionesAptas(Receta receta, IEnumerable<Restriccion> catalogo)
    {
        return catalogo
            .Where(r => EsApta(receta, [r.Id]))
            .Select(r => r.Nombre)
            .Order()
            .ToList();
    }

    // true si algún ingrediente tiene alguno de esos alérgenos (VN-05).
    public static bool ContieneAlergeno(Receta receta, ICollection<int> alergenosIds)
    {
        return receta.Ingredientes.Any(ri =>
            ri.Ingrediente.Alergenos.Any(a => alergenosIds.Contains(a.Id)));
    }

    // Cuántos de los gustos del usuario tiene la receta (para ordenar, VN-08).
    public static int GustosEnComun(Receta receta, ICollection<int> gustosIds)
    {
        return receta.Gustos.Count(g => gustosIds.Contains(g.Id));
    }

    // Lo que ve el usuario en el Inicio: sin sus alérgenos, aptas para todas sus restricciones,
    // primero las que más coinciden con sus gustos y, a igual coincidencia, por nombre.
    public static List<Receta> FiltrarYOrdenar(IEnumerable<Receta> recetas,
        ICollection<int> restriccionesIds, ICollection<int> alergenosIds, ICollection<int> gustosIds)
    {
        return recetas
            .Where(r => !ContieneAlergeno(r, alergenosIds))
            .Where(r => EsApta(r, restriccionesIds))
            .OrderByDescending(r => GustosEnComun(r, gustosIds))
            .ThenBy(r => r.Nombre)
            .ToList();
    }
}
