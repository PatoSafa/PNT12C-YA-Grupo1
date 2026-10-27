// Tests de las reglas de la despensa: porcentaje, "Podés cocinar" / "Casi listas" y búsqueda por sinónimo.
using CenaApp.Web.Models;
using CenaApp.Web.Services;

namespace CenaApp.Tests;

public class DespensaTests
{
    private static readonly Ingrediente Papa = new() { Id = 1, Nombre = "Papa", Sinonimos = "patata" };
    private static readonly Ingrediente Huevo = new() { Id = 2, Nombre = "Huevo" };
    private static readonly Ingrediente Cebolla = new() { Id = 3, Nombre = "Cebolla" };
    private static readonly Ingrediente Queso = new() { Id = 4, Nombre = "Queso" };
    private static readonly Ingrediente Palta = new() { Id = 5, Nombre = "Palta", Sinonimos = "aguacate" };
    private static readonly Ingrediente Limon = new() { Id = 6, Nombre = "Limón", Sinonimos = "lima" };
    private static readonly Ingrediente Aceite = new() { Id = 7, Nombre = "Aceite", CuentaEnDespensa = false };
    private static readonly Ingrediente Sal = new() { Id = 8, Nombre = "Sal", CuentaEnDespensa = false };

    private static Receta Receta(string nombre, params Ingrediente[] ingredientes) => new()
    {
        Nombre = nombre,
        Ingredientes = ingredientes.Select(i => new RecetaIngrediente { Ingrediente = i, IngredienteId = i.Id }).ToList(),
    };

    // ---------- Porcentaje (VN-10) ----------

    [Fact]
    public void Analizar_CuentaLosQueTieneYListaLosQueFaltan()
    {
        var tortilla = Receta("Tortilla", Papa, Huevo, Cebolla, Queso);

        var analisis = DespensaService.Analizar(tortilla, [Papa.Id, Huevo.Id, Cebolla.Id]);

        Assert.Equal(75, analisis.Porcentaje);
        Assert.Equal(["Queso"], analisis.Faltantes.Select(f => f.Ingrediente.Nombre));
    }

    [Fact]
    public void Analizar_NoCuentaLosBasicosComoAceiteOSal()
    {
        // Sin contar aceite y sal, tiene los 2 que cuentan: 100 %.
        var tortilla = Receta("Tortilla", Papa, Huevo, Aceite, Sal);

        var analisis = DespensaService.Analizar(tortilla, [Papa.Id, Huevo.Id]);

        Assert.Equal(100, analisis.Porcentaje);
        Assert.Empty(analisis.Faltantes);
    }

    [Fact]
    public void Analizar_DespensaVacia_EsCeroPorCiento()
    {
        var tortilla = Receta("Tortilla", Papa, Huevo);

        Assert.Equal(0, DespensaService.Analizar(tortilla, []).Porcentaje);
    }

    // ---------- Clasificación (VN-11 a VN-13) ----------

    [Fact]
    public void Clasificar_SeparaPodesCocinarYCasiListas_YDescartaLasDeMenosDel50()
    {
        var recetas = new[]
        {
            Receta("Puré", Papa),                          // 100 %
            Receta("Tortilla", Papa, Huevo, Cebolla),      // 66 %
            Receta("Papas con queso", Papa, Queso),        // 50 %
            Receta("Guacamole", Palta, Limon, Cebolla),    // 0 %
        };

        var resultado = DespensaService.Clasificar(recetas, [Papa.Id, Huevo.Id]);

        Assert.Equal(["Puré"], resultado.PodesCocinar.Select(a => a.Receta.Nombre));
        Assert.Equal(["Tortilla", "Papas con queso"], resultado.CasiListas.Select(a => a.Receta.Nombre));
    }

    [Fact]
    public void Clasificar_CasiListasVanDeMayorAMenorPorcentaje()
    {
        var recetas = new[]
        {
            Receta("A - mitad", Papa, Queso),                 // 50 %
            Receta("B - casi todo", Papa, Huevo, Cebolla, Queso), // 75 %
        };

        var resultado = DespensaService.Clasificar(recetas, [Papa.Id, Huevo.Id, Cebolla.Id]);

        Assert.Equal(["B - casi todo", "A - mitad"], resultado.CasiListas.Select(a => a.Receta.Nombre));
    }

    // ---------- Búsqueda por nombre o sinónimo (RF17) ----------

    [Theory]
    [InlineData("Palta")]
    [InlineData("palta")]
    [InlineData("  AGUACATE ")]
    public void Buscar_EncuentraPorNombreOSinonimoSinImportarMayusculas(string texto)
    {
        var encontrado = DespensaService.Buscar([Papa, Palta, Limon], texto);

        Assert.Equal("Palta", encontrado?.Nombre);
    }

    [Fact]
    public void Buscar_IgnoraLasTildes()
    {
        Assert.Equal("Limón", DespensaService.Buscar([Limon], "limon")?.Nombre);
    }

    [Fact]
    public void Buscar_SiNoEstaEnElCatalogo_DevuelveNull()
    {
        Assert.Null(DespensaService.Buscar([Papa, Palta], "kiwi"));
    }
}
