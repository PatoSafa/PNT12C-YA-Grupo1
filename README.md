# CenaAPP · PNT1 - 2C - YA - ORT · Grupo 1

**CenaAPP** es una app web, pensada para el celular, que recomienda recetas según el perfil de cada persona.

## Qué problema resuelve

A quien tiene restricciones alimentarias (vegetariana, vegana, sin gluten, sin lactosa, baja en sodio) o alergias, encontrar una receta le exige revisar los ingredientes uno por uno. CenaAPP lo hace por la persona:

- **Muestra solo las recetas aptas para su perfil.** Los alérgenos y las restricciones de cada receta se calculan a partir de sus ingredientes, no se cargan a mano.
- **Las ordena según sus gustos** (italiana, rápida, saludable…).
- **Despensa:** muestra qué se puede cocinar con lo que ya hay en casa y qué falta comprar.
- **Tiendas:** dónde comprar lo que falta, en un mapa con comercios de la zona.

## Tecnologías

| Para qué | Tecnología |
|---|---|
| Lenguaje y framework | C# / .NET 10, ASP.NET Core MVC con vistas Razor |
| Base de datos | PostgreSQL 17 en Docker, con Entity Framework Core (code-first y migraciones) |
| Estilos | Bootstrap + `wwwroot/css/estilos.css`; íconos con Bootstrap Icons |
| Mapa | Leaflet + OpenStreetMap |
| Tests | xUnit |
| Versionado | Git y GitHub |

Detalle y equivalencias con lo visto en clase (Spring Boot) en [`docs/TECNOLOGIAS.md`](docs/TECNOLOGIAS.md).

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Visual Studio Code con la extensión C# Dev Kit (opcional, para editar y usar `recetas.http`)

## Cómo ejecutarlo

```bash
# 1. Levantar PostgreSQL (la primera vez descarga la imagen)
docker compose up -d

# 2. Restaurar la herramienta de migraciones (solo la primera vez)
dotnet tool restore

# 3. Aplicar las migraciones (opcional: la app también las aplica al arrancar)
dotnet ef database update --project src/CenaApp.Web

# 4. Correr la app
dotnet run --project src/CenaApp.Web
```

Abrir http://localhost:5224.

Al arrancar por primera vez, la app carga los datos iniciales: 32 recetas, 81 ingredientes y los catálogos. También crea dos usuarios de prueba, que existen **solo en desarrollo**:

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin@cenaapp.com` | `admin1234` | Administrador |
| `demo@cenaapp.com` | `demo1234` | Usuario (vegetariana, alérgica a frutos secos, con despensa cargada) |

- Tests: `dotnet test`
- Apagar la base: `docker compose down`. Los datos se conservan en el volumen `cenaapp-datos`; para borrarlos, `docker compose down -v`.

## Despliegue en Render (Docker)

Render no tiene runtime nativo de .NET, así que la app se despliega como contenedor con el [`Dockerfile`](Dockerfile) de la raíz.

1. **Base de datos:** en Render, *New → PostgreSQL* (plan Free). Ojo: la base gratuita vence a los 30 días, conviene crearla cerca de la entrega. De la sección *Connections* anotar Hostname (interno), Database, Username y Password.
2. **Web Service:** *New → Web Service* → este repositorio → *Language: Docker* (usa el `Dockerfile` de la raíz) → plan Free.
3. **Variables de entorno** del Web Service (nunca en el repositorio):

   | Variable | Valor |
   |---|---|
   | `ConnectionStrings__CenaApp` | `Host=<hostname>;Port=5432;Database=<database>;Username=<username>;Password=<password>` |
   | `DatosIniciales__ClaveAdmin` | contraseña que elija el equipo para `admin@cenaapp.com` |
   | `DatosIniciales__ClaveDemo` | contraseña para `demo@cenaapp.com` |

   `PORT` la define Render: la app la lee al arrancar (`Program.cs`), igual que `server.port: ${PORT:8080}` en Spring.
4. **Deploy.** En el primer arranque la app aplica las migraciones y carga los datos iniciales. En el log aparece una vez `fail: ... Failed executing DbCommand ... __EFMigrationsHistory`. Es normal: EF Core busca la tabla de historial antes de crearla.

**Probar la imagen en local**, igual que en Render:
```bash
docker build -t cenaapp .
docker run --rm -p 10000:10000 -e PORT=10000   -e "ConnectionStrings__CenaApp=Host=host.docker.internal;Port=5432;Database=cenaapp;Username=cenaapp;Password=cenaapp_dev"   cenaapp
# http://localhost:10000
```

## API REST

Base: `http://localhost:5224/api/recetas`. Todas las pruebas están listas para ejecutar en [`src/CenaApp.Web/recetas.http`](src/CenaApp.Web/recetas.http).

| Método | Ruta | Qué hace | Respuestas |
|---|---|---|---|
| `GET` | `/api/recetas` | Lista todas las recetas | `200` |
| `GET` | `/api/recetas?nombre=papa` | Lista las que contienen ese texto en el nombre | `200` |
| `GET` | `/api/recetas/{id}` | Una receta con ingredientes, pasos y alérgenos calculados | `200` · `404` si no existe |
| `POST` | `/api/recetas` | Crea una receta | `201` · `400` con los errores de validación |
| `DELETE` | `/api/recetas/{id}` | Elimina una receta | `204` · `404` si no existe |

### Ejemplo: crear una receta

```http
POST /api/recetas
Content-Type: application/json
```
```json
{
  "nombre": "Ensalada de prueba",
  "descripcion": "Tomate y pepino",
  "emoji": "🥗",
  "tiempoMin": 10,
  "porciones": 2,
  "calorias": 150,
  "dificultad": "Facil",
  "categoriaId": 2,
  "ingredientes": [
    { "ingredienteId": 10, "cantidad": 100, "unidad": "g" },
    { "ingredienteId": 12, "cantidad": 50, "unidad": "g" }
  ],
  "pasos": ["Lavar y cortar las verduras.", "Mezclar y servir."]
}
```

`gustosIds` es opcional (por ejemplo `"gustosIds": [5, 9]`). Respuesta `201 Created`, con el encabezado `Location: /api/recetas/33`:
```json
{
  "id": 33,
  "nombre": "Ensalada de prueba",
  "categoria": "Ensalada",
  "dificultad": "Facil",
  "alergenos": [],
  "ingredientes": [
    { "nombre": "Tomate", "cantidad": 100, "unidad": "g" },
    { "nombre": "Pepino", "cantidad": 50, "unidad": "g" }
  ],
  "pasos": ["Lavar y cortar las verduras.", "Mezclar y servir."]
}
```

### Ejemplo: ID inexistente

`GET /api/recetas/9999` → `404 Not Found`
```json
{ "mensaje": "No existe una receta con id 9999." }
```

### Ejemplo: datos inválidos

Si el `POST` llega con el nombre vacío y el tiempo en 0, responde `400 Bad Request` con un error por campo:
```json
{
  "status": 400,
  "errors": {
    "Nombre": ["El nombre es obligatorio."],
    "TiempoMin": ["El tiempo tiene que estar entre 1 y 600 minutos."]
  }
}
```

## Documentación

| Archivo | Contenido |
|---|---|
| [`docs/CONSIGNA.md`](docs/CONSIGNA.md) | Requisitos del enunciado como checklist |
| [`docs/DECISIONES.md`](docs/DECISIONES.md) | Decisiones técnicas con alternativas y motivo (Evidencia 2) |
| [`docs/DISENO_ENTIDADES.md`](docs/DISENO_ENTIDADES.md) | Diseño de las entidades antes de programar (Evidencia 1) |
| [`docs/ERRORES.md`](docs/ERRORES.md) | Errores reales del desarrollo y cómo se resolvieron (Evidencia 3) |
| [`docs/ARQUITECTURA.md`](docs/ARQUITECTURA.md) | Capas, carpetas y modelo de datos |
| [`docs/PLAN.md`](docs/PLAN.md) | Tareas y avance |
| [`docs/TECNOLOGIAS.md`](docs/TECNOLOGIAS.md) | Stack y equivalencias con Spring Boot |
| [`docs/DISENO.md`](docs/DISENO.md) | Pantallas, colores y componentes (del prototipo de Figma) |
| [`docs/funcional/`](docs/funcional/) | Documentación funcional y guías de recetas y restricciones del equipo |
