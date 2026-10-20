// Contexto de EF Core: la puerta de entrada a la base de datos PostgreSQL.
using Microsoft.EntityFrameworkCore;

namespace CocinaFacil.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Los DbSet de cada entidad (Receta, Ingrediente, Usuario...) se agregan
    // cuando se implementa el modelo de datos (decisión 5), cada uno con su migración.
}
