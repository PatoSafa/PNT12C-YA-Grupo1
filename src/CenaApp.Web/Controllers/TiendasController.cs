// Tiendas cercanas: lista de comercios con filtro por tipo y mapa (Leaflet, D-11).
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers;

[Authorize]
public class TiendasController : Controller
{
    private readonly IComercioService _comercios;

    public TiendasController(IComercioService comercios)
    {
        _comercios = comercios;
    }

    // GET /Tiendas?tipo=Verdulería
    [HttpGet]
    public IActionResult Index(string? tipo)
    {
        var modelo = new TiendasViewModel
        {
            Tipo = tipo,
            Tipos = _comercios.Tipos(),
            Comercios = _comercios.Listar(tipo),
            Centro = _comercios.Centro,
        };
        return View(modelo);
    }
}
