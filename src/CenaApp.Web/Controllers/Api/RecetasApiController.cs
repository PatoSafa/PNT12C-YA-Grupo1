// API REST de recetas que pide el enunciado (Partes 8 a 12): GET, GET por id, POST y DELETE en /api/recetas.
// Devuelve JSON; las pantallas de la app usan controllers MVC con vistas Razor.
using CenaApp.Web.Models;
using CenaApp.Web.Services;
using CenaApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CenaApp.Web.Controllers.Api;

// [ApiController] valida las Data Annotations del DTO automáticamente: si algo no cumple, responde 400
// con la lista de errores sin llegar a ejecutar la acción.
[ApiController]
[Route("api/recetas")]
public class RecetasApiController : ControllerBase
{
    private readonly IRecetaService _recetas;

    public RecetasApiController(IRecetaService recetas)
    {
        _recetas = recetas;
    }

    // GET /api/recetas            → todas
    // GET /api/recetas?nombre=pap → las que contienen "pap" en el nombre
    [HttpGet]
    public async Task<ActionResult<List<RecetaResumenDto>>> Listar([FromQuery] string? nombre)
    {
        var recetas = await _recetas.ListarAsync(nombre);
        return recetas.Select(ADtoResumen).ToList();
    }

    // GET /api/recetas/5 → 200 con la receta, o 404 con un mensaje si no existe (Parte 10).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecetaDetalleDto>> Obtener(int id)
    {
        var receta = await _recetas.ObtenerAsync(id);
        if (receta is null)
        {
            return NotFound(new { mensaje = $"No existe una receta con id {id}." });
        }
        return ADtoDetalle(receta);
    }

    // POST /api/recetas → 201 con la receta creada y su URL, o 400 con los errores de validación.
    [HttpPost]
    public async Task<ActionResult<RecetaDetalleDto>> Crear(RecetaCrearDto datos)
    {
        var errores = await _recetas.ValidarAsync(datos);
        if (errores.Count > 0)
        {
            foreach (var (campo, mensaje) in errores)
            {
                ModelState.AddModelError(campo, mensaje);
            }
            return ValidationProblem(ModelState);
        }

        var creada = await _recetas.CrearAsync(datos);
        var completa = await _recetas.ObtenerAsync(creada.Id);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, ADtoDetalle(completa!));
    }

    // DELETE /api/recetas/5 → 204 si se borró, 404 si no existía (Parte 11).
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var borrada = await _recetas.EliminarAsync(id);
        if (!borrada)
        {
            return NotFound(new { mensaje = $"No existe una receta con id {id}." });
        }
        return NoContent();
    }

    private static RecetaResumenDto ADtoResumen(Receta r) =>
        new(r.Id, r.Nombre, r.Emoji, r.Categoria.Nombre, r.TiempoMin, r.Porciones, r.Dificultad, r.Calorias);

    private static RecetaDetalleDto ADtoDetalle(Receta r) =>
        new(r.Id, r.Nombre, r.Descripcion, r.Emoji, r.Categoria.Nombre, r.TiempoMin, r.Porciones, r.Dificultad, r.Calorias,
            r.Gustos.Select(g => g.Nombre).Order().ToList(),
            RecetaService.CalcularAlergenos(r),
            r.Ingredientes.Select(ri => new IngredienteDeRecetaDto(ri.Ingrediente.Nombre, ri.Cantidad, ri.Unidad)).ToList(),
            r.Pasos.OrderBy(p => p.Orden).Select(p => p.Texto).ToList());
}
