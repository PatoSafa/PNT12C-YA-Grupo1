// Contexto de EF Core: la puerta de entrada a la base de datos PostgreSQL.
// Cada DbSet es una tabla; OnModelCreating define las relaciones, índices únicos y qué pasa al borrar.
using CenaApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CenaApp.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<RecetaIngrediente> RecetaIngredientes => Set<RecetaIngrediente>();
    public DbSet<Paso> Pasos => Set<Paso>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Restriccion> Restricciones => Set<Restriccion>();
    public DbSet<Alergeno> Alergenos => Set<Alergeno>();
    public DbSet<Gusto> Gustos => Set<Gusto>();

    // Los enum se guardan como texto ("Facil") en vez de número (0): la base se lee sin traducir.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Rol>().HaveConversion<string>();
        configurationBuilder.Properties<Nivel>().HaveConversion<string>();
        configurationBuilder.Properties<Dificultad>().HaveConversion<string>();
        configurationBuilder.Properties<TipoGusto>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índices únicos
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Receta>().HasIndex(r => r.Nombre).IsUnique();
        modelBuilder.Entity<Ingrediente>().HasIndex(i => i.Nombre).IsUnique();
        modelBuilder.Entity<Categoria>().HasIndex(c => c.Nombre).IsUnique();

        // Receta → Categoria (N:1). No se puede borrar una categoría que tiene recetas.
        modelBuilder.Entity<Receta>()
            .HasOne(r => r.Categoria)
            .WithMany()
            .HasForeignKey(r => r.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Receta → Paso (1:N). Al borrar la receta se borran sus pasos.
        modelBuilder.Entity<Receta>()
            .HasMany(r => r.Pasos)
            .WithOne()
            .HasForeignKey(p => p.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Receta ↔ Ingrediente con cantidad (N:M con clase propia). La clave es el par (receta, ingrediente):
        // un ingrediente no puede repetirse en la misma receta (VAL-18).
        modelBuilder.Entity<RecetaIngrediente>().HasKey(ri => new { ri.RecetaId, ri.IngredienteId });
        modelBuilder.Entity<RecetaIngrediente>()
            .HasOne(ri => ri.Receta)
            .WithMany(r => r.Ingredientes)
            .HasForeignKey(ri => ri.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);
        // No se puede borrar un ingrediente que se usa en alguna receta (VN-21).
        modelBuilder.Entity<RecetaIngrediente>()
            .HasOne(ri => ri.Ingrediente)
            .WithMany()
            .HasForeignKey(ri => ri.IngredienteId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RecetaIngrediente>().Property(ri => ri.Cantidad).HasPrecision(8, 2);

        // Relaciones N:M sin clase propia: EF Core crea la tabla intermedia.
        // Se indica el nombre de la tabla y de sus dos columnas para que se lean claras en la base.
        TablaIntermedia(modelBuilder.Entity<Usuario>().HasMany(u => u.Restricciones).WithMany(),
            "UsuarioRestricciones", typeof(Usuario), "UsuarioId", typeof(Restriccion), "RestriccionId");
        TablaIntermedia(modelBuilder.Entity<Usuario>().HasMany(u => u.Alergenos).WithMany(),
            "UsuarioAlergenos", typeof(Usuario), "UsuarioId", typeof(Alergeno), "AlergenoId");
        TablaIntermedia(modelBuilder.Entity<Usuario>().HasMany(u => u.Gustos).WithMany(),
            "UsuarioGustos", typeof(Usuario), "UsuarioId", typeof(Gusto), "GustoId");
        TablaIntermedia(modelBuilder.Entity<Usuario>().HasMany(u => u.Despensa).WithMany(),
            "Despensa", typeof(Usuario), "UsuarioId", typeof(Ingrediente), "IngredienteId");
        TablaIntermedia(modelBuilder.Entity<Usuario>().HasMany(u => u.Favoritos).WithMany(),
            "Favoritos", typeof(Usuario), "UsuarioId", typeof(Receta), "RecetaId");
        TablaIntermedia(modelBuilder.Entity<Ingrediente>().HasMany(i => i.Alergenos).WithMany(),
            "IngredienteAlergenos", typeof(Ingrediente), "IngredienteId", typeof(Alergeno), "AlergenoId");
        TablaIntermedia(modelBuilder.Entity<Ingrediente>().HasMany(i => i.RestriccionesIncompatibles).WithMany(),
            "IngredienteRestriccionesIncompatibles", typeof(Ingrediente), "IngredienteId", typeof(Restriccion), "RestriccionId");
        TablaIntermedia(modelBuilder.Entity<Receta>().HasMany(r => r.Gustos).WithMany(),
            "RecetaGustos", typeof(Receta), "RecetaId", typeof(Gusto), "GustoId");
    }

    // Configura una tabla intermedia N:M: "duenio" es la entidad que tiene la lista (ej: Usuario)
    // y "relacionada" la del otro lado (ej: Receta). Al borrar cualquiera de las dos, se borra la fila de la tabla.
    private static void TablaIntermedia(CollectionCollectionBuilder relacion, string tabla,
        Type duenio, string columnaDuenio, Type relacionada, string columnaRelacionada)
    {
        relacion.UsingEntity(tabla,
            ladoRelacionado => ladoRelacionado.HasOne(relacionada).WithMany().HasForeignKey(columnaRelacionada),
            ladoDuenio => ladoDuenio.HasOne(duenio).WithMany().HasForeignKey(columnaDuenio));
    }
}
