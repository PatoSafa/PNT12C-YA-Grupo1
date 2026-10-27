# Guía para la defensa oral y la prueba sorpresa

Para preparar la Evidencia 6 (explicación oral) y la Parte 19 (modificación en vivo). Cualquier integrante tiene que poder explicar cualquier archivo, así que conviene que cada uno practique los tres recorridos de abajo.

---

## 1. Tres recorridos completos

### A. `GET /api/recetas/6` (API, Partes 8 a 10)
1. **`Controllers/Api/RecetasApiController.cs` → `Obtener(int id)`.** `[Route("api/recetas")]` y `[HttpGet("{id:int}")]` hacen que esta acción atienda la petición. Equivale a `@GetMapping("/{id}")` + `@PathVariable`.
2. **El controller le pide la receta a `IRecetaService`**, que llega por constructor (inyección de dependencias, registrada en `Program.cs` con `AddScoped`).
3. **`Services/RecetaService.cs` → `ObtenerAsync`** usa `_db.Recetas.Include(...)`: EF Core arma el SQL con los JOIN a categoría, gustos, pasos e ingredientes.
4. **`Data/AppDbContext.cs`**: el `DbSet<Receta>` es la tabla `Recetas` de PostgreSQL (Docker, `compose.yaml`).
5. **Vuelta:** si es `null`, responde `NotFound(new { mensaje })` → **404** (D-17). Si existe, la convierte en `RecetaDetalleDto` (con los alérgenos calculados por `CalcularAlergenos`) → **200** con JSON.

### B. Inicio de un usuario (MVC con Razor)
1. **`RecetasController.Index(buscar, gusto)`**, con `[Authorize]`: sin sesión, redirige al login (configurado en `Program.cs` → `AddCookie`).
2. **`UsuarioActual.Id(User)`** lee el id desde la cookie de sesión (claim `NameIdentifier`, guardado en `CuentaController.IniciarSesionAsync`).
3. **`RecetaService.ListarParaUsuarioAsync`** carga el perfil y las recetas, y llama a **`FiltrarYOrdenar`**: saca las recetas con alérgenos del usuario, saca las que tienen un ingrediente incompatible con sus restricciones y ordena por gustos en común.
4. **El controller arma `InicioRecetasViewModel` y hace `return View(modelo)`.**
5. **`Views/Recetas/Index.cshtml`** dibuja el HTML; cada tarjeta es la vista parcial `_TarjetaReceta`.

### C. Agregar un ingrediente a la despensa (AJAX, Parte 15)
1. **`Views/Despensa/Index.cshtml`**: el formulario `#form-agregar`.
2. **`wwwroot/js/despensa.js`** cancela el envío normal (`preventDefault`) y hace `fetch(POST /Despensa/Agregar)` con `FormData`, el token antiforgery y el encabezado `X-Requested-With: fetch`.
3. **`DespensaController.Agregar`** → `DespensaService.AgregarAsync`, que busca por nombre o sinónimo (`Buscar`, sin tildes) y agrega la fila en la tabla `Despensa`.
4. **`ResponderAsync`**: como vino de `fetch`, devuelve `PartialView("_Contenido", ...)`, que es solo el HTML del bloque.
5. **`despensa.js`** pone ese HTML en `#contenido-despensa` (`innerHTML`). La página no se recarga.

---

## 2. Preguntas probables por archivo

| Archivo | ¿Qué hace? | ¿Qué pasa si se borra o se cambia? |
|---|---|---|
| `Program.cs` | Arma la app: registra los servicios (DI), la base, el login por cookie y el orden de los middleware | Si falta `AddScoped<IRecetaService, RecetaService>`, los controllers que lo piden fallan con "Unable to resolve service". Si `UseAuthentication` va después de `UseAuthorization`, nadie queda logueado |
| `Data/AppDbContext.cs` | Una tabla por `DbSet`; en `OnModelCreating`, relaciones, índices únicos y qué pasa al borrar | Cambiar algo acá requiere una migración nueva (`dotnet ef migrations add ...`) |
| `Data/Migrations/*_Inicial.cs` | El SQL (en C#) que crea las tablas; queda versionado en Git | Si se borra, la base no se puede crear desde cero |
| `Models/Receta.cs` | La entidad: cada propiedad es una columna y cada `List<>` es una relación | Ver la receta de "nuevo atributo" abajo |
| `ViewModels/RecetaDtos.cs` | Lo que entra y sale de la API, con las validaciones (`[Required]`, `[Range]`) | Sin `[Range(1, …)]`, la API aceptaría tiempo 0 |
| `Services/RecetaService.cs` | Las reglas: aptitud, alérgenos, alta, baja y favoritos | Es lo que prueban los tests (`dotnet test`) |
| `Controllers/Api/RecetasApiController.cs` | Los 4 endpoints REST | `[ApiController]` hace que una validación fallida devuelva 400 sola, sin entrar a la acción |
| `Views/Shared/_Layout.cshtml` | El esqueleto común y la barra inferior | Todas las vistas lo usan (`_ViewStart.cshtml`) |
| `wwwroot/css/estilos.css` | Colores de Figma en `:root` que pisan a Bootstrap, y los componentes propios | Cambiar `--color-primario` cambia el naranja de toda la app |
| `compose.yaml` | Levanta PostgreSQL 17 con volumen | Sin el volumen, los datos se pierden al hacer `docker compose down` |
| `Dockerfile` | La imagen para Render, en dos etapas | Render no tiene .NET nativo: sin esto no hay deploy |
| `Data/DatosIniciales.cs` | Carga catálogos, recetas y usuarios de prueba si la base está vacía | Solo corre con la base vacía: los cambios no se aplican a una base que ya tiene datos |

**Preguntas de concepto que conviene tener claras:**
- **¿Qué problema nos evita resolver `JpaRepository` (en .NET, `DbSet`)?** No hay que escribir SQL ni el CRUD de cada entidad a mano.
- **¿Por qué `Receta` es una entidad?** Es algo del dominio que se guarda y tiene identidad propia (`Id`).
- **¿Qué relación hay entre la clase y la tabla?** EF Core mapea la clase a la tabla `Recetas`: cada objeto es una fila y cada propiedad, una columna. La tabla se crea con una migración.
- **¿Por qué esa relación entre `Receta` y `Categoria`?** Muchas recetas tienen una categoría (N:1). En la base es la columna `CategoriaId` con clave foránea, y `Restrict` impide borrar una categoría que está en uso.
- **¿Por qué `RecetaIngrediente` tiene clase y `Favoritos` no?** `RecetaIngrediente` guarda datos propios (cantidad y unidad). `Favoritos` solo une dos ids, y EF Core crea esa tabla sola.

---

## 3. Prueba sorpresa: los ejemplos del enunciado

### "Agregá un nuevo atributo y hacé que aparezca en el POST y en el GET"
Ejemplo: `Origen` (texto) en Receta.
1. `Models/Receta.cs` → agregar `[MaxLength(40)] public string? Origen { get; set; }`.
2. Terminal: `dotnet ef migrations add AgregarOrigenAReceta --project src/CenaApp.Web --output-dir Data/Migrations`. Después, `dotnet ef database update --project src/CenaApp.Web`, o simplemente arrancar la app, que aplica la migración sola.
3. `ViewModels/RecetaDtos.cs`:
   - en `RecetaCrearDto`, agregar `[StringLength(40)] public string? Origen { get; set; }`;
   - en `RecetaDetalleDto`, agregar `string? Origen` como parámetro.
4. `Services/RecetaService.cs` → `CrearAsync`: `Origen = datos.Origen,` (y lo mismo en `ActualizarAsync`).
5. `RecetasApiController.ADtoDetalle` → pasar `r.Origen` en la posición nueva.
6. Probar con `recetas.http`: un POST con `"origen": "Italia"` y después el GET de esa receta.

### "Un endpoint que filtre por nombre que contenga una palabra"
Ya existe: `GET /api/recetas?nombre=papa` (`RecetasApiController.Listar` → `RecetaService.ListarAsync`). Si piden otro filtro, por ejemplo por categoría, se sigue el mismo patrón:
- agregar `[FromQuery] int? categoriaId` en `Listar`;
- en `ListarAsync`, agregar `if (categoriaId is not null) consulta = consulta.Where(r => r.CategoriaId == categoriaId);`.

### "Modificá la respuesta cuando no existe el ID"
En `RecetasApiController.Obtener` está `return NotFound(new { mensaje = ... })`. Por ejemplo, para sumar el id buscado: `NotFound(new { mensaje = "...", idBuscado = id })`. En las pantallas está en `RecetasController.Detalle` y en la vista `Views/Recetas/NoEncontrada.cshtml`.

---

## 4. Comandos para tener a mano

```bash
docker compose up -d                          # base
dotnet run --project src/CenaApp.Web          # app → http://localhost:5224
dotnet test                                   # tests (21)
dotnet ef migrations add <Nombre> --project src/CenaApp.Web --output-dir Data/Migrations
dotnet ef database update --project src/CenaApp.Web
docker compose down -v                        # borra la base (vuelve a cargarse sola al arrancar)
```

Usuarios de prueba (solo en desarrollo): `admin@cenaapp.com` / `admin1234` y `demo@cenaapp.com` / `demo1234`.
