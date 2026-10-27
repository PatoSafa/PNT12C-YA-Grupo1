// Comercios de ejemplo alrededor de ORT Yatay (Almagro). Nombres ficticios en direcciones reales de la zona (D-15);
// las coordenadas de cada dirección salen de OpenStreetMap (Nominatim).
// Para reemplazarlos por un servicio de mapas real, alcanza con cambiar esta clase: la vista y el JS no cambian.
namespace CenaApp.Web.Services;

public class ComercioService : IComercioService
{
    public (double Lat, double Lng, string Nombre) Centro => (-34.60995, -58.42914, "ORT Yatay"); // Yatay 240

    // Datos de ejemplo (sin distancia: se calcula al listar).
    private static readonly (string Nombre, string Tipo, string Emoji, string Direccion, string Horario, double Lat, double Lng)[] Datos =
    [
        ("Súper La Esquina", "Supermercado", "🛒", "Av. Rivadavia 4500", "Lun a Sáb 8 a 21 h · Dom 9 a 14 h", -34.61485, -58.42839),
        ("Verdulería Don Ramón", "Verdulería", "🥬", "Yatay 420", "Lun a Sáb 7 a 20 h", -34.60689, -58.42927),
        ("Carnicería El Novillo", "Carnicería", "🥩", "Av. Corrientes 4300", "Lun a Sáb 8 a 13 y 17 a 20:30 h", -34.60282, -58.42549),
        ("Dietética Semillas", "Dietética", "🌾", "Bogado 4400", "Lun a Vie 9 a 19 h · Sáb 9 a 13 h", -34.60520, -58.42760),
        ("Almacén Doña Rosa", "Almacén", "🏪", "Pringles 410", "Todos los días 8 a 22 h", -34.60706, -58.42775),
        ("Panadería La Espiga", "Panadería", "🥖", "Yatay 150", "Todos los días 7 a 21 h", -34.61177, -58.42883),
        ("Pescadería Puerto Sur", "Pescadería", "🐟", "Av. Rivadavia 4650", "Mar a Sáb 8 a 14 h", -34.61606, -58.43106),
    ];

    public List<string> Tipos() => Datos.Select(d => d.Tipo).Distinct().Order().ToList();

    public List<Comercio> Listar(string? tipo = null)
    {
        return Datos
            .Select((d, i) => new Comercio(i + 1, d.Nombre, d.Tipo, d.Emoji, d.Direccion, d.Horario, d.Lat, d.Lng,
                DistanciaKm(Centro.Lat, Centro.Lng, d.Lat, d.Lng)))
            .Where(c => string.IsNullOrEmpty(tipo) || c.Tipo == tipo)
            .OrderBy(c => c.DistanciaKm)
            .ToList();
    }

    // Distancia en línea recta entre dos puntos (fórmula del haversine), redondeada a 0,1 km.
    public static double DistanciaKm(double lat1, double lng1, double lat2, double lng2)
    {
        const double radioTierraKm = 6371;
        double ARadianes(double grados) => grados * Math.PI / 180;

        var dLat = ARadianes(lat2 - lat1);
        var dLng = ARadianes(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ARadianes(lat1)) * Math.Cos(ARadianes(lat2)) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var distancia = radioTierraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Math.Round(distancia, 1);
    }
}
