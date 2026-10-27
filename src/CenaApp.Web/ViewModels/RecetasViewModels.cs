// Datos que reciben las vistas de recetas (Inicio, tarjetas y detalle).
using CenaApp.Web.Models;

namespace CenaApp.Web.ViewModels;

// Una tarjeta de receta del listado (vista parcial _TarjetaReceta).
public class TarjetaRecetaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public string Emoji { get; set; } = "";
    public int TiempoMin { get; set; }
    public int Porciones { get; set; }
    public Dificultad Dificultad { get; set; }
    public bool EsFavorita { get; set; }
}

// Pantalla de Inicio: saludo, buscador, filtro por gusto y tarjetas.
public class InicioRecetasViewModel
{
    public string NombreUsuario { get; set; } = "";
    public string? Buscar { get; set; }
    public int? GustoId { get; set; }
    public string? GustoElegido { get; set; }
    public List<Gusto> Gustos { get; set; } = [];
    public List<TarjetaRecetaViewModel> Recetas { get; set; } = [];
}
