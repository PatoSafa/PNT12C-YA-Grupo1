# Tecnologías del proyecto

Stack de CenaAPP y su equivalencia con lo que se vio en clase (Java + Spring Boot).

> Cuando el equipo apruebe este archivo ("TECNOLOGIAS.md aprobado"), pasa a ser la fuente de verdad del stack y reemplaza la tabla de la sección 3 de `CLAUDE.md`.

## 1. Stack base

| Capa | Tecnología | Equivalente visto en clase | Dónde se vio |
|---|---|---|---|
| Lenguaje y runtime | C# sobre .NET 10 (LTS) | Java 17 | Clase 3 — guía Spring Boot + Gradle |
| Framework | ASP.NET Core MVC | Spring Boot (Web) | Clase 3 — guía Spring Boot + Gradle |
| Controller | Clases `Controller` con `[HttpGet]`, `[HttpPost]`, `[Route]` | `@RestController`, `@GetMapping` | Clase 3; Clase 5 — CRUD |
| View | Vistas Razor (`.cshtml`) con `_Layout.cshtml` compartido | `static/*.html` | Clase 4 — guía completa local/Render; Clase 5 |
| View, partes interactivas | JavaScript con `fetch()` contra endpoints JSON | `fetch()` / `$.ajax` | Clase 2 — AJAX; Clase 3 — frontend/backend; Clase 5 — login AJAX |
| Estilos | Bootstrap + `wwwroot/css/estilos.css` propio | CSS externo en `css/estilos.css` | Clase 2 — repaso HTML/CSS/JS |
| Model, datos | Entity Framework Core, code-first con migraciones | Spring Data JPA | Clase 3; Enunciado Partes 6 y 7 |
| Model, reglas | Clases *Service* con interfaz, registradas en DI | Capa `Service` | Clase 5 — CRUD de componentes |
| Model hacia la View | ViewModels / DTOs | — (en clase se devolvía la entidad) | No visto (ver sección 3) |
| Validación | Data Annotations | Spring Validation | Clase 3 (dependencia); Enunciado Parte 12 |
| Base de datos | PostgreSQL (Npgsql) en Docker | MySQL en Docker (local), PostgreSQL en Render | Clases 3 y 4 (MySQL); Clase 6 (PostgreSQL en prod) |
| Configuración | `appsettings.json` + `appsettings.Development.json` + variables de entorno / User Secrets | `application.yaml` + perfiles `local`/`prod` | Clase 4; Clase 6 |
| Login y roles | Cookie auth + `PasswordHasher<T>` + `[Authorize(Roles = "Admin")]` | `sessionStorage` + `window.location.href` | Clase 5 — login AJAX |
| Build | `dotnet` CLI, `.sln` / `.csproj` | Gradle (Clase 3), Maven (Clases 4–6) | Clases 3–6 |
| Tests | xUnit | `src/test` | Enunciado — estructura de entrega |
| Probar endpoints | Postman o archivos `.http` | Postman / Insomnia | Clases 3, 4 y 5 |
| Contenedores | `compose.yaml` (PostgreSQL) + `Dockerfile` | `compose.yaml` (MySQL) | Clase 3; Clase 4 |
| Despliegue | Render con `Dockerfile` | Render con runtime Java nativo | Clase 4; Clase 6 |
| Versionado | Git, GitHub | Git, GitHub | Clase 3; Clase 4 — guía de conexión |

También habilitadas por la cátedra: Node.js y npm (solo como herramientas de desarrollo).

## 2. Equivalencias Spring → .NET, concepto por concepto

| Concepto | Spring (como se vio) | .NET (como lo hacemos) | Clase de origen |
|---|---|---|---|
| Crear el proyecto | Spring Initializr con Web, Data JPA, MySQL Driver, Validation | `dotnet new mvc` + paquetes `Npgsql.EntityFrameworkCore.PostgreSQL` y `Microsoft.EntityFrameworkCore.Design` | Clase 3 — guía Spring Boot + Gradle |
| Archivo de build | `build.gradle` / `pom.xml` | `.csproj` (proyecto) y `.sln` (solución) | Clase 3 (Gradle); Clases 4–6 (Maven) |
| Correr la app | `./gradlew bootRun` / `./mvnw spring-boot:run` | `dotnet run --project src/CenaApp.Web` | Clase 3; Clase 4 |
| Empaquetar | `./mvnw clean package` → `.jar` | `dotnet publish -c Release` | Clase 6 |
| Carpetas | `controller/`, `model/`, `repository/` | `Controllers/`, `Models/`, `Data/`, `Services/`, `Views/` | Clase 3; Clase 4 |
| Entidad | `@Entity`, `@Id`, `@GeneratedValue(IDENTITY)` | Clase C# común + `DbSet<T>` en el `DbContext`; `Id` es la clave por convención | Clase 3; Enunciado Parte 6 |
| Repository | `interface XRepository extends JpaRepository<X, Long>` | `DbContext` / `DbSet<T>`: `ToListAsync()`, `FindAsync(id)`, `Add` + `SaveChangesAsync()`, `Remove` | Clase 3; Enunciado Parte 7 |
| Crear/actualizar tablas | `spring.jpa.hibernate.ddl-auto=update` | Migraciones: `dotnet ef migrations add <Nombre>` + `dotnet ef database update` | Clases 3, 4 y 6 |
| Ver el SQL | `show-sql: true` | Nivel de log `Microsoft.EntityFrameworkCore.Database.Command` en `appsettings.Development.json` | Clase 4; Clase 6 |
| Relaciones | Producto → Categoría (desafío extra) | Propiedades de navegación + FK; `Include()` para traerlas | Clase 3 (desafío); Enunciado (desafío extra) |
| Controller REST | `@RestController` + `@RequestMapping("/api/x")` | `[ApiController]` + `[Route("api/x")]` sobre `ControllerBase` | Clase 3 — guías |
| Controller que devuelve vista | `@Controller` (+ `@ResponseBody` para JSON) | `Controller` con `return View(modelo)` o `return Json(...)` | Clase 5 — login AJAX |
| Verbos HTTP | `@GetMapping`, `@PostMapping`, `@PutMapping`, `@DeleteMapping` | `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` | Clase 3 (GET/POST/DELETE); Clase 5 (PUT) |
| Parámetros | `@RequestBody`, `@PathVariable`, `@RequestParam` | `[FromBody]`, `{id}` en la ruta, `[FromQuery]` | Clase 3; Clase 5 |
| Respuesta con código | `ResponseEntity.ok()`, `.accepted()`, 404 "Componente no encontrado" | `IActionResult`: `Ok()`, `Accepted()`, `NotFound("Receta no encontrada")`, `CreatedAtAction()` | Clase 5 — CRUD de componentes |
| ID inexistente | No dejar que falle; decidir y explicar | `if (receta is null) return NotFound(...)` o vista "no encontrado" | Enunciado Partes 10, 11 y 19 |
| Capa de servicio | `@Service` llamado desde el controller | `IRecetaService` + `RecetaService`, registrado con `builder.Services.AddScoped<...>()` | Clase 5 |
| Inyección de dependencias | Inyección por constructor | Inyección por constructor (la registra `Program.cs`) | Clase 3 |
| Validación | Dependencia Validation, `@NotBlank`, `@Min`, `@Email` | `[Required]`, `[Range]`, `[EmailAddress]`, `[StringLength]` + `ModelState.IsValid` | Clase 3; Enunciado Parte 12 |
| Búsqueda por query | `GET /api/libros?autor=...` | `[FromQuery] string? nombre` + LINQ `Where(...Contains(...))` | Enunciado Partes 14 y 19 |
| Páginas | `src/main/resources/static/index.html` | Vistas Razor en `Views/` + estáticos en `wwwroot/` | Clase 4; Clase 5 |
| CSS / JS propios | `css/estilos.css`, `js/script.js` | `wwwroot/css/estilos.css`, `wwwroot/js/*.js` | Clase 2 — repaso |
| AJAX | `fetch` GET/POST con `Content-Type: application/json` y `JSON.stringify`; `$.ajax` | Igual, contra acciones que devuelven JSON | Clase 2; Clase 3; Clase 5 |
| URLs relativas | `fetch("/elementos")`, nunca `localhost:8080` | Igual (`fetch("/api/recetas")` o `@Url.Action(...)`) | Clase 4; Clase 6 |
| Mostrar éxito y error en la página | Mensajes distintos, no solo `console.log` | Revisar `response.ok` y mostrar alerta de Bootstrap | Clase 2 — AJAX; Práctica 2 |
| CORS | `@CrossOrigin` | No hace falta: vistas y API salen de la misma app | Clase 3 — frontend/backend |
| API Gateway | Concepto: un punto de entrada que rutea por recurso | Concepto equivalente: una sola app con rutas `/api/...` | Clase 2 — AJAX |
| CRUD en pantalla | Tabla con +/editar/eliminar, confirmación, recargar | Vistas Index/Create/Edit + confirmación antes de borrar | Clase 5 — CRUD de componentes |
| Login | POST `/login` por AJAX, usuario en `sessionStorage` | Cookie de autenticación firmada por el servidor + `PasswordHasher` (la guía de clase aclara que `sessionStorage` no es autenticación real) | Clase 5 — login AJAX |
| Configuración | `application.properties` / `application.yaml` | `appsettings.json` (`ConnectionStrings:DefaultConnection`) | Clase 3; Clase 4 |
| Perfiles | `application-local.yaml`, `application-prod.yaml`, `SPRING_PROFILES_ACTIVE` | `appsettings.Development.json`, `appsettings.Production.json`, `ASPNETCORE_ENVIRONMENT` | Clase 6 |
| Secretos | `${DB_URL}`, `${DB_USERNAME}`, `${DB_PASSWORD}` | Variable `ConnectionStrings__DefaultConnection` o User Secrets en local | Clase 4; Clase 6 |
| Puerto | `server.port: ${PORT:8080}` | Leer `PORT` en `Program.cs` / `ASPNETCORE_URLS` | Clase 4; Clase 6 |
| Base en Docker | `compose.yaml` con `mysql:8.4`, puerto `3306:3306`, volumen | `compose.yaml` con `postgres`, puerto `5432:5432`, volumen | Clase 3; Clase 4 |
| Orden de prueba | Docker → backend → Postman → página | Igual | Clase 4 — guía de conexión |
| Deploy | Render Web Service, runtime Java, PostgreSQL de Render | Render Web Service con runtime Docker (`Dockerfile`), PostgreSQL de Render | Clase 4; Clase 6 |
| Git | `init -b main`, `remote add`, `add/commit/push`, `clone` | Igual, con `.gitignore` de .NET (`bin/`, `obj/`) | Clase 3; Clase 4 |
| Commits | Al menos 4 significativos, no uno solo al final | Igual | Enunciado Parte 16 |
| README | Descripción, tecnologías, cómo correr, endpoints, JSON de ejemplo | Igual, con comandos `docker compose` / `dotnet` | Clase 3; Enunciado Parte 17 |

## 3. Conceptos que no se vieron en clase (preparar para la defensa)

| Concepto | Por qué lo usamos | Qué había en clase |
|---|---|---|
| **Vistas Razor** | Es la "V" del MVC de ASP.NET Core: el Controller arma el modelo y la vista lo dibuja en el servidor | HTML estático que pedía datos por `fetch` |
| **Migraciones de EF Core** | Cada cambio del modelo queda versionado en Git, en vez de que la base se modifique sola | `ddl-auto=update` |
| **ViewModels / DTOs** | Evitan mandar a la vista datos que no corresponden (por ejemplo `PasswordHash`) | Se devolvía la entidad directamente |
| **Dockerfile** | Render no tiene runtime nativo de .NET, así que la app se despliega como contenedor | Render compilaba Java por su cuenta; Docker solo se usaba para la base |
| **Cookie auth + `PasswordHasher`** | Login real del lado del servidor, con contraseñas hasheadas | `sessionStorage` (la guía aclaraba que no es seguro) |
| **xUnit** | Tests automáticos del proyecto | No hubo ejemplos de tests |

## 4. Tecnologías complementarias

Para sumar una tiene que pasar los cuatro filtros de `CLAUDE.md` (resuelve algo concreto, complementa sin reemplazar, es chica y aislada, el equipo la puede explicar). Se propone como decisión, y si se aprueba queda en `docs/DECISIONES.md` y en esta tabla.

| Tecnología | Para qué | Estado |
|---|---|---|
| Leaflet + OpenStreetMap | Mapa de Tiendas con comercios fijos | **Aprobada** (D-11) |
| Bootstrap Icons | Íconos (el prototipo usa lucide) | **Aprobada** (D-10) |

**No usar sin decisión explícita:** React, TypeScript, Tailwind, shadcn, ASP.NET Core Identity completo, Blazor, Java/Spring Boot.
