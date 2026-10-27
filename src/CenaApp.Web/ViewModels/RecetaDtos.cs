// Objetos que entran y salen de la API /api/recetas. Se usan en lugar de la entidad para no exponer
// el modelo de la base y para validar lo que llega en el POST.
using System.ComponentModel.DataAnnotations;
using CenaApp.Web.Models;

namespace CenaApp.Web.ViewModels;

// Lo que devuelve GET /api/recetas (listado).
public record RecetaResumenDto(
    int Id,
    string Nombre,
    string Emoji,
    string Categoria,
    int TiempoMin,
    int Porciones,
    Dificultad Dificultad,
    int Calorias);

// Lo que devuelve GET /api/recetas/{id} (detalle).
public record RecetaDetalleDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string Emoji,
    string Categoria,
    int TiempoMin,
    int Porciones,
    Dificultad Dificultad,
    int Calorias,
    List<string> Gustos,
    List<string> Alergenos,
    List<IngredienteDeRecetaDto> Ingredientes,
    List<string> Pasos);

public record IngredienteDeRecetaDto(string Nombre, decimal? Cantidad, string? Unidad);

// Lo que recibe POST /api/recetas. Las Data Annotations son las validaciones (Parte 12 del enunciado).
public class RecetaCrearDto
{
    // VAL-15: obligatorio, hasta 80 caracteres (la unicidad se controla en el servicio).
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre puede tener hasta 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [StringLength(300, ErrorMessage = "La descripción puede tener hasta 300 caracteres.")]
    public string? Descripcion { get; set; }

    [StringLength(16)]
    public string? Emoji { get; set; }

    // VAL-16: tiempo y porciones enteros mayores a 0; calorías mayores o iguales a 0.
    [Range(1, 600, ErrorMessage = "El tiempo tiene que estar entre 1 y 600 minutos.")]
    public int TiempoMin { get; set; }

    [Range(1, 50, ErrorMessage = "Las porciones tienen que estar entre 1 y 50.")]
    public int Porciones { get; set; }

    [Range(0, 5000, ErrorMessage = "Las calorías tienen que estar entre 0 y 5000.")]
    public int Calorias { get; set; }

    [Required(ErrorMessage = "La dificultad es obligatoria (Facil, Media o Dificil).")]
    public Dificultad? Dificultad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoría es obligatoria.")]
    public int CategoriaId { get; set; }

    // VAL-18: al menos un ingrediente.
    [MinLength(1, ErrorMessage = "La receta necesita al menos un ingrediente.")]
    public List<IngredienteCrearDto> Ingredientes { get; set; } = [];

    // VAL-19: al menos un paso.
    [MinLength(1, ErrorMessage = "La receta necesita al menos un paso.")]
    public List<string> Pasos { get; set; } = [];
}

public class IngredienteCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Falta el ingrediente.")]
    public int IngredienteId { get; set; }

    // null = "a gusto"
    [Range(0.01, 10000, ErrorMessage = "La cantidad tiene que ser mayor a 0.")]
    public decimal? Cantidad { get; set; }

    [StringLength(20)]
    public string? Unidad { get; set; }
}
