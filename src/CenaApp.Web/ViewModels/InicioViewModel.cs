// Datos que recibe la vista de inicio provisoria: quién está logueado y si la app llega a la base de datos.
namespace CenaApp.Web.ViewModels;

public class InicioViewModel
{
    public string Nombre { get; set; } = "";
    public bool EsAdmin { get; set; }
    public bool BaseConectada { get; set; }
}
