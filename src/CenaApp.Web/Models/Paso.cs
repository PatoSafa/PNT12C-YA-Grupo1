// Un paso numerado de una receta.
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public class Paso
{
    public int Id { get; set; }

    public int RecetaId { get; set; }

    public int Orden { get; set; }

    [Required, MaxLength(500)]
    public string Texto { get; set; } = "";
}
