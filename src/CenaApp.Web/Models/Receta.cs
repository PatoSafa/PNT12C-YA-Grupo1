// Receta del catálogo. Sus alérgenos y restricciones aptas no se guardan: se calculan desde los ingredientes.
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public class Receta
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = "";

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Required, MaxLength(16)]
    public string Emoji { get; set; } = "🍽️";

    public int TiempoMin { get; set; }

    public int Porciones { get; set; }

    public Dificultad Dificultad { get; set; }

    // Estimadas, por porción.
    public int Calorias { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public List<Paso> Pasos { get; set; } = [];
    public List<RecetaIngrediente> Ingredientes { get; set; } = [];
    public List<Gusto> Gustos { get; set; } = [];
}
