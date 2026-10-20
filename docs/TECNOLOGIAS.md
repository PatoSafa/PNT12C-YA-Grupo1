# Tecnologías

Stack del proyecto y su equivalencia con lo visto en las clases (Java + Spring Boot). Las clases se citan por archivo dentro de `_referencias/clases/`.

> **Estado:** borrador para validar. Cuando el equipo escriba "TECNOLOGIAS.md aprobado", esta lista reemplaza la tabla de la sección 3 de `CLAUDE.md`.

## Lo que se vio en clase

| Clase | Archivo | Temas |
|---|---|---|
| 1 | `Clase 1/Clase1.html`, `Practica clase 1 - Entrega 19-08_.docx` | HTML desde cero: estructura, títulos, listas, `div`, formularios (`form`, `label`, `input`, `select`, `textarea`, `button`), tablas, etiquetas semánticas |
| 2 | `Clase 2/Clase2 - repaso_html_css_javascript.html` | CSS (en línea, interno y **externo**), JavaScript: funciones, eventos, modificar el DOM, archivo `.js` externo |
| 2 | `Clase 2/Clase2 - ajax_api_gateway_backend.html`, `Practica 2 - Entrega 07-09_.docx` | AJAX con `fetch()`, métodos HTTP, `POST` con `method`, `headers`, `Content-Type: application/json`, `body` y `JSON.stringify()`, API Gateway, manejo de errores |
| 3 | `Clase 3/repaso_javascript_interactiva_codigo.html` | Variables, condicionales, validación de formularios en JavaScript, `console.log()` |
| 3 | `Clase 3/guia_springboot_gradle_docker_github.html` | Java 17, Spring Initializr, **Gradle (Groovy)**, dependencias Spring Web, Spring Data JPA, MySQL Driver y Validation, MySQL `mysql:8.4` en Docker, `@Entity`, `@Id`, `@GeneratedValue`, `JpaRepository`, `@RestController`, `@RequestMapping`, `@GetMapping`, `@PostMapping`, `@RequestBody`, `ddl-auto`, Postman/Insomnia, Git y GitHub |
| 3 | `Clase 3/guia_frontend_backend_springboot.html` | Conectar frontend y backend: `fetch()`, CORS y `@CrossOrigin`, `POST`, `DELETE` con `@DeleteMapping` y `@PathVariable` |
| 4 | `Clase 4/guia_completa_springboot_local_render.html`, `guia_conexion_backend_vista_git_docker.html` | Levantar en local, vista en `static/index.html`, variables `DB_USER` / `DB_PASSWORD`, despliegue en Render, puerto `${PORT}`, Maven, Git Bash (config, clone, commit, push), errores frecuentes |
| 5 | `Clase 5/guia_crud_componentes.html` | CRUD completo desde la vista: `GET`, `POST`, `PUT`, `DELETE`, `ResponseEntity`, `@RequestParam`, recargar la tabla |
| 5 | `Clase 5/paso_a_paso_home_ajax.html` | Login con AJAX, **jQuery 3.7.1** con `$.ajax`, usuario guardado en `sessionStorage`, redirección al Home |
| 6 | `Clase 6/preparaProyectoRender.html` | Preparar para Render: **PostgreSQL**, perfiles `application-local.yaml` / `application-prod.yaml`, variables de entorno, `server.port=${PORT:8080}`, Maven, `render.yaml` |

## Stack y equivalencias

| Capa | Nuestro proyecto (.NET) | Equivalente en clase (Spring) | Visto en |
|---|---|---|---|
| Lenguaje y runtime | C# sobre .NET 10 | Java 17 | Clase 3 |
| Framework | ASP.NET Core MVC | Spring Boot (Spring Web) | Clase 3 |
| Crear el proyecto | `dotnet new mvc` / `dotnet new sln` | Spring Initializr | Clase 3 |
| Build y dependencias | `dotnet` CLI, `.sln` / `.csproj`, paquetes NuGet | Gradle (Groovy) / Maven | Clases 3, 4, 6 |
| Controller (pantallas) | Clase `Controller` + `return View(viewModel)` | `static/index.html` + controller REST | Clases 3, 4 |
| Controller (API) | `[ApiController]`, `[Route]`, `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`, `[FromBody]`, parámetros de ruta y de query, `ActionResult` / `NotFound()` | `@RestController`, `@RequestMapping`, `@GetMapping`, `@PostMapping`, `@PutMapping`, `@DeleteMapping`, `@RequestBody`, `@PathVariable`, `@RequestParam`, `ResponseEntity` | Clases 3, 5 |
| Vista | Vistas Razor (`.cshtml`) con `_Layout.cshtml` | HTML en `static/` | Clases 1, 4 |
| Estilos | Bootstrap + `wwwroot/css/estilos.css` propio | CSS externo | Clase 2 |
| JavaScript | Archivos en `wwwroot/js/`, `fetch()` con JSON y `JSON.stringify()` | `fetch()`, `$.ajax` | Clases 2, 3, 5 |
| CORS | No hace falta: vista y API se sirven desde la misma app | `@CrossOrigin` | Clase 3 |
| Entidad | Clase en `Models/` con propiedad `Id` (clave autogenerada por convención) | `@Entity`, `@Id`, `@GeneratedValue` | Clase 3 |
| Acceso a datos | `DbContext` + `DbSet<T>` (EF Core) | `JpaRepository<T, ID>` | Clase 3 |
| Esquema de la base | Migraciones (`dotnet ef migrations add`, `dotnet ef database update`) | `spring.jpa.hibernate.ddl-auto` | Clase 3 |
| Validación | Data Annotations: `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]` | Dependencia Validation (anotaciones Bean Validation) | Clase 3 / Enunciado |
| Base de datos | PostgreSQL 17 en Docker (local y producción) | MySQL 8.4 en Docker (local), PostgreSQL (Render) | Clases 3, 6 |
| Configuración | `appsettings.json` + `appsettings.Development.json` + variables de entorno (`ConnectionStrings__CocinaFacil`) | `application.yaml` + perfiles `local` / `prod` + variables de entorno | Clases 4, 6 |
| Puerto en producción | Variable `ASPNETCORE_HTTP_PORTS` / `PORT` en el contenedor | `server.port=${PORT:8080}` | Clases 4, 6 |
| Login y sesión | Autenticación por cookie + `PasswordHasher<T>` + `[Authorize(Roles = "Admin")]` | Login con AJAX + usuario en `sessionStorage` | Clase 5 |
| Tests | xUnit (`tests/CocinaFacil.Tests`) | `src/test` | Clase 3 (estructura) |
| Probar endpoints | Postman, Insomnia o archivos `.http` | Postman, Insomnia | Clases 3, 4 |
| Contenedores | Docker Desktop, `compose.yaml` | Docker Desktop, `compose.yaml` | Clases 3, 4 |
| Despliegue | Render con `Dockerfile` (Render no tiene runtime nativo de .NET) | Render (Web Service + PostgreSQL) | Clases 4, 6 |
| Versionado | Git, GitHub, Git Bash | Git, GitHub, Git Bash | Clases 3, 4 |
| IDE | Visual Studio Code + C# Dev Kit + Claude Code | IntelliJ IDEA | Clase 3 |

## Lo que usamos y no se vio en clase

Habilitado por la cátedra (ecosistema .NET, JavaScript, Node.js, Docker y Bootstrap), pero conviene poder explicarlo porque no tiene guía de clase:

| Elemento | Por qué lo usamos | Cómo explicarlo en la defensa |
|---|---|---|
| Capa de Services | Las reglas de negocio (alérgenos, aptitud, despensa) quedan en un solo lugar (D-02) | En clase el Controller llamaba directo al Repository; nosotros agregamos un paso intermedio |
| Vistas Razor y ViewModels | MVC de ASP.NET Core: el Controller arma el ViewModel y la View lo muestra (D-06) | Equivale a armar el HTML en el servidor en lugar de traer JSON con `fetch()` |
| Migraciones de EF Core | Cada cambio del modelo queda versionado en Git | Reemplaza a `ddl-auto` |
| Autenticación por cookie y `PasswordHasher` | Login, roles y contraseñas con hash sin librerías externas | Reemplaza al `sessionStorage` de la Clase 5, que no es seguro para roles |
| Relaciones entre entidades | El modelo tiene recetas, ingredientes, pasos, alérgenos y restricciones | Es el "Desafío extra" del enunciado |
| jQuery | Viene con la plantilla MVC para la validación del lado del cliente | Visto en la Clase 5; decisión pendiente si se mantiene |

## Complementarias

Ninguna aprobada todavía. Candidata: Leaflet + OpenStreetMap para el mapa de Tiendas (segunda entrega, D-09).
