// Catálogos fijos que se cargan con los datos iniciales: categorías, restricciones, alérgenos y gustos.
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Nombre { get; set; } = "";
}

public class Restriccion
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Nombre { get; set; } = "";

    [MaxLength(200)]
    public string? Descripcion { get; set; }
}

public class Alergeno
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Nombre { get; set; } = "";
}

public class Gusto
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Nombre { get; set; } = "";

    public TipoGusto Tipo { get; set; }
}
