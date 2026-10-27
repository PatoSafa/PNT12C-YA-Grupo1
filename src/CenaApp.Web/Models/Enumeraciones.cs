// Valores fijos del dominio. En la base se guardan como texto (ver AppDbContext.ConfigureConventions).
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public enum Rol
{
    Usuario,
    Admin
}

public enum Nivel
{
    Principiante,
    Intermedio,
    Avanzado
}

public enum Dificultad
{
    // Display: cómo se muestra en los <select> (el valor guardado sigue siendo "Facil").
    [Display(Name = "Fácil")]
    Facil,
    Media,
    [Display(Name = "Difícil")]
    Dificil
}

public enum TipoGusto
{
    Cocina,
    Estilo
}
