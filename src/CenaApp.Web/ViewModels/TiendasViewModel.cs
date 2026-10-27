// Datos que recibe la pantalla de Tiendas.
using CenaApp.Web.Services;

namespace CenaApp.Web.ViewModels;

public class TiendasViewModel
{
    public string? Tipo { get; set; }
    public List<string> Tipos { get; set; } = [];
    public List<Comercio> Comercios { get; set; } = [];
    public (double Lat, double Lng, string Nombre) Centro { get; set; }
}
