// Valores fijos del dominio. En la base se guardan como texto (ver AppDbContext.ConfigureConventions).
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
    Facil,
    Media,
    Dificil
}

public enum TipoGusto
{
    Cocina,
    Estilo
}
