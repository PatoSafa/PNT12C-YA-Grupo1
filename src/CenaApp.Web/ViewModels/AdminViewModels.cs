// Datos de las pantallas de administración (ingredientes y recetas).
using System.ComponentModel.DataAnnotations;
using CenaApp.Web.Models;

namespace CenaApp.Web.ViewModels;

// ---------- Ingredientes ----------

public class IngredienteFilaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Grupo { get; set; } = "";
    public string? Sinonimos { get; set; }
    public List<string> Alergenos { get; set; } = [];
    public int RecetasQueLoUsan { get; set; }
}

public class IngredienteFormularioViewModel
{
    // null = alta; con valor = edición.
    public int? Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre puede tener hasta 60 caracteres.")]
    public string Nombre { get; set; } = "";

    // VAL-23
    [Required(ErrorMessage = "El grupo es obligatorio.")]
    [StringLength(40, ErrorMessage = "El grupo puede tener hasta 40 caracteres.")]
    public string Grupo { get; set; } = "";

    // VAL-24: separados por coma (cada uno de hasta 30 caracteres, se controla en el servicio).
    [StringLength(200, ErrorMessage = "Los sinónimos pueden tener hasta 200 caracteres en total.")]
    [Display(Name = "Sinónimos")]
    public string? Sinonimos { get; set; }

    [Display(Name = "Cuenta en el porcentaje de la despensa")]
    public bool CuentaEnDespensa { get; set; } = true;

    public List<int> AlergenosIds { get; set; } = [];
    public List<int> RestriccionesIncompatiblesIds { get; set; } = [];

    // Opciones para dibujar el formulario (las carga el controller).
    public List<Alergeno> Alergenos { get; set; } = [];
    public List<Restriccion> Restricciones { get; set; } = [];
    public List<string> Grupos { get; set; } = [];
}

// ---------- Recetas ----------

public class RecetaFilaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Categoria { get; set; } = "";
    public int TiempoMin { get; set; }
}

// Formulario de alta y edición. Los datos son el mismo DTO que recibe la API (POST /api/recetas),
// así las validaciones y el alta están en un solo lugar (RecetaService).
public class RecetaFormularioViewModel
{
    // Unidades permitidas para las cantidades (VAL-18).
    public static readonly string[] Unidades = ["g", "kg", "ml", "l", "ud", "cda", "cdita", "taza", "pizca"];

    // null = alta; con valor = edición.
    public int? Id { get; set; }

    public RecetaCrearDto Datos { get; set; } = new();

    // Los pasos se escriben en un textarea, uno por línea; el controller los separa en Datos.Pasos.
    [Display(Name = "Pasos (uno por línea)")]
    public string PasosTexto { get; set; } = "";

    // Solo en edición: alérgenos que resultan de los ingredientes guardados (vista previa, RF8).
    public List<string> AlergenosCalculados { get; set; } = [];

    // Opciones para dibujar el formulario (las carga el controller).
    public List<Categoria> Categorias { get; set; } = [];
    public List<Gusto> Gustos { get; set; } = [];
    public List<Ingrediente> Ingredientes { get; set; } = [];
}

