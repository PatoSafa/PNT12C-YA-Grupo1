// Pantalla de inicio provisoria: confirma que la app está corriendo y que llega a la base.
using System.Diagnostics;
using CocinaFacil.Web.Data;
using CocinaFacil.Web.Models;
using CocinaFacil.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CocinaFacil.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = new InicioViewModel
        {
            BaseConectada = await _db.Database.CanConnectAsync()
        };
        return View(modelo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
