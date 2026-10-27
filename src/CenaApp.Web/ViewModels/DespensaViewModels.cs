// Datos que reciben la pantalla de Despensa y su vista parcial de resultados.
using CenaApp.Web.Models;

namespace CenaApp.Web.ViewModels;

// Página completa: buscador, accesos rápidos y el bloque de resultados.
public class DespensaViewModel
{
    // Para el <datalist> del buscador: nombre y sinónimos de cada ingrediente del catálogo.
    public List<(string Nombre, string? Sinonimos)> Catalogo { get; set; } = [];
    public DespensaContenidoViewModel Contenido { get; set; } = new();
}

// Lo que se actualiza con fetch (vista parcial _Contenido): la despensa y las recetas sugeridas.
public class DespensaContenidoViewModel
{
    // Error al agregar (ingrediente que no existe o repetido). null si salió bien.
    public string? Mensaje { get; set; }
    public List<Ingrediente> Ingredientes { get; set; } = [];
    // Accesos rápidos: ingredientes comunes que todavía no están en la despensa (RF17).
    public List<Ingrediente> Rapidos { get; set; } = [];
    public List<RecetaDespensaViewModel> PodesCocinar { get; set; } = [];
    public List<RecetaDespensaViewModel> CasiListas { get; set; } = [];
}

public class RecetaDespensaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Emoji { get; set; } = "";
    public int TiempoMin { get; set; }
    public Dificultad Dificultad { get; set; }
    public int Porcentaje { get; set; }
    // "Queso fresco (30 g)"
    public List<string> Faltantes { get; set; } = [];
}
