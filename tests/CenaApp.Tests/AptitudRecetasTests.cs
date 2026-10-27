// Tests de las reglas de aptitud de RecetaService: alérgenos calculados, restricciones y orden por gustos.
using CenaApp.Web.Models;
using CenaApp.Web.Services;

namespace CenaApp.Tests;

public class AptitudRecetasTests
{
    // ---------- Datos de prueba (objetos en memoria, sin base de datos) ----------
    private static readonly Restriccion Vegetariana = new() { Id = 1, Nombre = "Vegetariana" };
    private static readonly Restriccion Vegana = new() { Id = 2, Nombre = "Vegana" };
    private static readonly Restriccion SinGluten = new() { Id = 3, Nombre = "Sin gluten" };

    private static readonly Alergeno Gluten = new() { Id = 1, Nombre = "Gluten" };
    private static readonly Alergeno Lacteos = new() { Id = 2, Nombre = "Lácteos" };
    private static readonly Alergeno Huevo = new() { Id = 3, Nombre = "Huevo" };

    private static readonly Gusto Italiana = new() { Id = 1, Nombre = "Italiana" };
    private static readonly Gusto Rapido = new() { Id = 2, Nombre = "Rápido" };

    private static readonly Ingrediente Tomate = new() { Id = 1, Nombre = "Tomate" };
    private static readonly Ingrediente Pollo = new() { Id = 2, Nombre = "Pollo", RestriccionesIncompatibles = [Vegetariana, Vegana] };
    private static readonly Ingrediente Queso = new() { Id = 3, Nombre = "Queso", Alergenos = [Lacteos], RestriccionesIncompatibles = [Vegana] };
    private static readonly Ingrediente HuevoIng = new() { Id = 4, Nombre = "Huevo", Alergenos = [Huevo], RestriccionesIncompatibles = [Vegana] };
    private static readonly Ingrediente Fideos = new() { Id = 5, Nombre = "Fideos", Alergenos = [Gluten], RestriccionesIncompatibles = [SinGluten] };

    private static Receta Receta(string nombre, Ingrediente[] ingredientes, params Gusto[] gustos) => new()
    {
        Nombre = nombre,
        Ingredientes = ingredientes.Select(i => new RecetaIngrediente { Ingrediente = i, IngredienteId = i.Id }).ToList(),
        Gustos = gustos.ToList(),
    };

    // ---------- Alérgenos (VN-04) ----------

    [Fact]
    public void CalcularAlergenos_JuntaLosDeTodosLosIngredientesSinRepetir()
    {
        var pasta = Receta("Pasta con queso y huevo", [Fideos, Queso, HuevoIng, Queso]);

        var alergenos = RecetaService.CalcularAlergenos(pasta);

        Assert.Equal(["Gluten", "Huevo", "Lácteos"], alergenos);
    }

    [Fact]
    public void CalcularAlergenos_RecetaSinAlergenos_DevuelveListaVacia()
    {
        var ensalada = Receta("Ensalada", [Tomate]);

        Assert.Empty(RecetaService.CalcularAlergenos(ensalada));
    }

    // ---------- Restricciones (VN-06) ----------

    [Fact]
    public void EsApta_ConUnIngredienteIncompatible_NoEsApta()
    {
        var polloConTomate = Receta("Pollo con tomate", [Pollo, Tomate]);

        Assert.False(RecetaService.EsApta(polloConTomate, [Vegetariana.Id]));
    }

    [Fact]
    public void EsApta_SinIngredientesIncompatibles_EsApta()
    {
        var tortilla = Receta("Tortilla", [HuevoIng, Tomate]);

        Assert.True(RecetaService.EsApta(tortilla, [Vegetariana.Id]));
    }

    [Fact]
    public void EsApta_ConVariasRestricciones_TieneQueCumplirTodas()
    {
        // Apta para vegetariana, pero no para vegana (por el queso).
        var tarta = Receta("Tarta de queso", [Queso, Tomate]);

        Assert.True(RecetaService.EsApta(tarta, [Vegetariana.Id]));
        Assert.False(RecetaService.EsApta(tarta, [Vegetariana.Id, Vegana.Id]));
    }

    [Fact]
    public void EsApta_UsuarioSinRestricciones_TodoEsApto()
    {
        var pollo = Receta("Pollo", [Pollo]);

        Assert.True(RecetaService.EsApta(pollo, []));
    }

    [Fact]
    public void RestriccionesAptas_DevuelveLasQueLaRecetaCumple()
    {
        var tarta = Receta("Tarta de queso", [Queso, Tomate]);

        var aptas = RecetaService.RestriccionesAptas(tarta, [Vegetariana, Vegana, SinGluten]);

        Assert.Equal(["Sin gluten", "Vegetariana"], aptas);
    }

    // ---------- Lo que ve el usuario en el Inicio (VN-05, VN-06, VN-08) ----------

    [Fact]
    public void FiltrarYOrdenar_OcultaLasRecetasConAlergenosDelUsuario()
    {
        var recetas = new[] { Receta("Ensalada", [Tomate]), Receta("Tarta de queso", [Queso]) };

        var visibles = RecetaService.FiltrarYOrdenar(recetas, [], [Lacteos.Id], []);

        Assert.Equal(["Ensalada"], visibles.Select(r => r.Nombre));
    }

    [Fact]
    public void FiltrarYOrdenar_OcultaLasRecetasNoAptasParaSusRestricciones()
    {
        var recetas = new[] { Receta("Ensalada", [Tomate]), Receta("Pollo", [Pollo]) };

        var visibles = RecetaService.FiltrarYOrdenar(recetas, [Vegetariana.Id], [], []);

        Assert.Equal(["Ensalada"], visibles.Select(r => r.Nombre));
    }

    [Fact]
    public void FiltrarYOrdenar_PrimeroLasQueMasCoincidenConSusGustos_YDespuesPorNombre()
    {
        var recetas = new[]
        {
            Receta("Arroz", [Tomate]),                          // 0 gustos en común
            Receta("Ñoquis", [Tomate], Italiana),               // 1
            Receta("Pizza rápida", [Tomate], Italiana, Rapido), // 2
            Receta("Bruschetta", [Tomate], Italiana),           // 1
        };

        var visibles = RecetaService.FiltrarYOrdenar(recetas, [], [], [Italiana.Id, Rapido.Id]);

        Assert.Equal(["Pizza rápida", "Bruschetta", "Ñoquis", "Arroz"], visibles.Select(r => r.Nombre));
    }
}
