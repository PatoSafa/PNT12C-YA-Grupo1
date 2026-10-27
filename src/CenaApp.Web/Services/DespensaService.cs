// Despensa: ingredientes que el usuario tiene en casa y cálculo de qué recetas puede cocinar (RF16 a RF18).
using System.Globalization;
using System.Text;
using CenaApp.Web.Data;
using CenaApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CenaApp.Web.Services;

public class DespensaService : IDespensaService
{
    // VN-11 / VN-12: desde qué porcentaje una receta aparece en "Casi listas" (D-03, punto 8).
    public const int MinimoCasiListas = 50;

    private readonly AppDbContext _db;

    public DespensaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Ingrediente>> ObtenerAsync(int usuarioId)
    {
        return await _db.Usuarios
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.Despensa)
            .OrderBy(i => i.Nombre)
            .ToListAsync();
    }

    public async Task<List<Ingrediente>> CatalogoAsync()
    {
        return await _db.Ingredientes.OrderBy(i => i.Nombre).ToListAsync();
    }

    public async Task<string?> AgregarAsync(int usuarioId, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "Escribí un ingrediente.";
        }

        var ingrediente = Buscar(await CatalogoAsync(), texto);
        if (ingrediente is null)
        {
            return $"\"{texto.Trim()}\" no está en el catálogo de ingredientes.";
        }

        var usuario = await _db.Usuarios.Include(u => u.Despensa).FirstAsync(u => u.Id == usuarioId);
        if (usuario.Despensa.Any(i => i.Id == ingrediente.Id))
        {
            return $"{ingrediente.Nombre} ya está en tu despensa.";
        }

        usuario.Despensa.Add(ingrediente);
        await _db.SaveChangesAsync();
        return null;
    }

    public async Task QuitarAsync(int usuarioId, int ingredienteId)
    {
        var usuario = await _db.Usuarios.Include(u => u.Despensa).FirstAsync(u => u.Id == usuarioId);
        usuario.Despensa.RemoveAll(i => i.Id == ingredienteId);
        await _db.SaveChangesAsync();
    }

    public async Task VaciarAsync(int usuarioId)
    {
        var usuario = await _db.Usuarios.Include(u => u.Despensa).FirstAsync(u => u.Id == usuarioId);
        usuario.Despensa.Clear();
        await _db.SaveChangesAsync();
    }

    public async Task<ResultadoDespensa> SugerenciasAsync(int usuarioId)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Despensa)
            .Include(u => u.Restricciones)
            .Include(u => u.Alergenos)
            .AsSplitQuery()
            .FirstAsync(u => u.Id == usuarioId);

        var recetas = await _db.Recetas
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.Alergenos)
            .Include(r => r.Ingredientes).ThenInclude(ri => ri.Ingrediente).ThenInclude(i => i.RestriccionesIncompatibles)
            .AsSplitQuery()
            .ToListAsync();

        // Las mismas reglas que el Inicio: no se sugiere nada que el usuario no pueda comer.
        var aptas = RecetaService.FiltrarYOrdenar(recetas,
            usuario.Restricciones.Select(r => r.Id).ToList(),
            usuario.Alergenos.Select(a => a.Id).ToList(),
            []);

        return Clasificar(aptas, usuario.Despensa.Select(i => i.Id).ToHashSet());
    }

    // ---------------------------------------------------------------------------------------------
    // Reglas de la despensa. Funciones puras (sin base de datos), con tests en CenaApp.Tests.
    // ---------------------------------------------------------------------------------------------

    // Porcentaje de ingredientes que el usuario tiene, sin contar los básicos como sal o agua (VN-10),
    // y la lista de los que faltan. Una receta hecha solo de básicos cuenta como 100 %.
    public static AnalisisReceta Analizar(Receta receta, ICollection<int> despensaIds)
    {
        var queCuentan = receta.Ingredientes.Where(ri => ri.Ingrediente.CuentaEnDespensa).ToList();
        var faltantes = queCuentan.Where(ri => !despensaIds.Contains(ri.IngredienteId)).ToList();

        var porcentaje = queCuentan.Count == 0
            ? 100
            : (queCuentan.Count - faltantes.Count) * 100 / queCuentan.Count;

        return new AnalisisReceta(receta, porcentaje, faltantes);
    }

    // 100 % → "Podés cocinar", por nombre. 50–99 % → "Casi listas", de mayor a menor porcentaje (VN-11 a VN-13).
    public static ResultadoDespensa Clasificar(IEnumerable<Receta> recetas, ICollection<int> despensaIds)
    {
        var analisis = recetas.Select(r => Analizar(r, despensaIds)).ToList();

        var podesCocinar = analisis
            .Where(a => a.Porcentaje == 100)
            .OrderBy(a => a.Receta.Nombre)
            .ToList();

        var casiListas = analisis
            .Where(a => a.Porcentaje >= MinimoCasiListas && a.Porcentaje < 100)
            .OrderByDescending(a => a.Porcentaje)
            .ThenBy(a => a.Receta.Nombre)
            .ToList();

        return new ResultadoDespensa(podesCocinar, casiListas);
    }

    // Busca un ingrediente del catálogo por nombre o por sinónimo, sin distinguir mayúsculas ni tildes
    // ("palta", "Aguacate" y "AGUACATE" encuentran la Palta) (RF17).
    public static Ingrediente? Buscar(IEnumerable<Ingrediente> catalogo, string texto)
    {
        var buscado = Normalizar(texto);
        return catalogo.FirstOrDefault(i =>
            Normalizar(i.Nombre) == buscado
            || (i.Sinonimos ?? "").Split(',').Any(s => Normalizar(s) == buscado));
    }

    // "  Limón " → "limon": sin espacios de más, en minúsculas y sin tildes.
    public static string Normalizar(string texto)
    {
        var descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sinTildes = descompuesto.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(sinTildes.ToArray()).Normalize(NormalizationForm.FormC);
    }
}
