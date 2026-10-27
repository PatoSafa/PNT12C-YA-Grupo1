// Un ingrediente dentro de una receta, con su cantidad. Tiene clase propia porque guarda datos de la relación.
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public class RecetaIngrediente
{
    public int RecetaId { get; set; }
    public Receta Receta { get; set; } = null!;

    public int IngredienteId { get; set; }
    public Ingrediente Ingrediente { get; set; } = null!;

    // null = "a gusto". decimal porque hay cantidades como 0,5 huevo.
    public decimal? Cantidad { get; set; }

    // Ej: "g", "ml", "ud", "cda".
    [MaxLength(20)]
    public string? Unidad { get; set; }

    [MaxLength(100)]
    public string? Nota { get; set; }
}
