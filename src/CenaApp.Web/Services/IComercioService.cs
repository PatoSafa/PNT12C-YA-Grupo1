// Contrato del servicio de comercios: la lista fija de comercios de ejemplo para la pantalla Tiendas.
namespace CenaApp.Web.Services;

public interface IComercioService
{
    // Punto desde el que se miden las distancias y donde se centra el mapa (ORT Yatay, D-15).
    (double Lat, double Lng, string Nombre) Centro { get; }

    // Tipos de comercio, para los filtros.
    List<string> Tipos();

    // Comercios del tipo pedido (o todos), del más cercano al más lejano (RF20).
    List<Comercio> Listar(string? tipo = null);
}

// Un comercio. No es una entidad: no se guarda en la base (la doc funcional dice que los comercios
// salen del servicio de mapas; por ahora son una lista fija, D-04 y D-08).
public record Comercio(
    int Id,
    string Nombre,
    string Tipo,
    string Emoji,
    string Direccion,
    string Horario,
    double Lat,
    double Lng,
    double DistanciaKm);
