# Decisiones del proyecto

Registro de las decisiones de arquitectura (Evidencia 2 — Decisiones del enunciado).
Formato: fecha, decisión, alternativas y motivo.

---

## D-01 · Stack: C# / .NET 10 con ASP.NET Core MVC
- **Fecha:** 19/10/2026
- **Decisión:** C# sobre .NET 10, ASP.NET Core MVC con vistas Razor, Bootstrap, Entity Framework Core y PostgreSQL. Detalle en la sección 3 de `CLAUDE.md`.
- **Alternativas:** Java + Spring Boot (el de las clases).
- **Motivo:** la cátedra habilitó .NET y su ecosistema. Ver "Por qué este stack" en `CLAUDE.md`.

## D-02 · Patrón: MVC con capa de Services y ViewModels
- **Fecha:** 19/10/2026
- **Decisión:** Controllers delgados, reglas de negocio en Services y ViewModels/DTOs hacia la View.
- **Alternativas:** lógica dentro de los Controllers.
- **Motivo:** separa responsabilidades y deja las reglas (alérgenos, aptitud, despensa) en un solo lugar testeable.

## D-03 · Alcance de la primera entrega
- **Fecha:** 19/10/2026
- **Decisión:** entran registro y login con roles, perfil (onboarding y edición), recetas filtradas por alérgenos y restricciones, detalle de receta, despensa ("Podés cocinar" / "Casi listas"), favoritos y ABM de recetas para el administrador. Quedan para la segunda entrega: fotos de los platos, mapa embebido de comercios, ABM de ingredientes y restricciones o alergias personalizadas.
- **Alternativas:** implementar toda la documentación funcional en la primera entrega.
- **Motivo:** cubre los casos de uso centrales y los mínimos del enunciado sin poner en riesgo el plazo.

## D-04 · Acceso a datos: los Services usan el DbContext directamente
- **Fecha:** 19/10/2026
- **Decisión:** los Services reciben `AppDbContext` por inyección de dependencias y consultan los `DbSet` con LINQ. No hay capa Repository.
- **Alternativas:** capa Repository encima de EF Core (calca el Controller → Service → Repository de Spring visto en clase).
- **Motivo:** EF Core ya funciona como repositorio (`DbSet`) y unidad de trabajo (`SaveChanges`). Una capa extra duplicaría código sin agregar valor. En la defensa: el `DbSet<Receta>` cumple el rol del `JpaRepository<Receta>` de Spring.

## D-05 · Organización del código: carpetas por capa
- **Fecha:** 19/10/2026
- **Decisión:** `Controllers/`, `Models/`, `ViewModels/`, `Services/`, `Data/`, `Views/`, `wwwroot/`.
- **Alternativas:** carpetas por módulo (`Recetas/`, `Usuarios/`…).
- **Motivo:** es la convención de ASP.NET Core MVC (las vistas se buscan en `Views/<Controller>/`) y coincide con la estructura de referencia de `CLAUDE.md`.

## D-06 · Razor por defecto, fetch solo donde suma
- **Fecha:** 19/10/2026
- **Decisión:** las pantallas se resuelven con vistas Razor (el Controller arma el ViewModel y devuelve la View). Se usa `fetch()` contra endpoints JSON solo para agregar o quitar ingredientes de la despensa y para marcar favoritos. El checklist de pasos de una receta es JavaScript local, sin servidor.
- **Alternativas:** toda la app con `fetch()` contra una API JSON, como el prototipo de Figma.
- **Motivo:** cada pantalla queda como un MVC "de libro", fácil de defender, y `fetch()` se usa donde evita recargas que molestan al usuario.

## D-07 · Modelo de datos de la primera entrega
- **Fecha:** 19/10/2026
- **Decisión:** se implementan las entidades de la sección 9 de la documentación funcional, salvo `PreferenciaPersonalizada` (segunda entrega). `RecetaIngrediente` es una entidad explícita porque guarda cantidad, unidad y nota; las demás relaciones muchos a muchos las resuelve EF Core. Los alérgenos y las restricciones de cada receta se calculan a partir de sus ingredientes en `RecetaService`.
- **Alternativas:** guardar los alérgenos como texto en cada receta.
- **Motivo:** el cálculo desde los ingredientes evita alérgenos mal cargados (errores detectados en el prototipo) y es la regla VN-04 de la documentación funcional.

## D-08 · Estilos: Bootstrap + estilos.css con los colores de Figma
- **Fecha:** 19/10/2026
- **Decisión:** Bootstrap para la grilla, tarjetas y formularios. Los colores, fuentes y tamaños del prototipo van como variables CSS en `:root` de `wwwroot/css/estilos.css`, que pisa a Bootstrap. Contenedor de 512 px como máximo.
- **Alternativas:** CSS propio completo, sin Bootstrap.
- **Motivo:** rapidez con Bootstrap y un único archivo para ajustar el diseño a mano.

## D-09 · Mapa de comercios: links a Google Maps en la primera entrega
- **Fecha:** 19/10/2026
- **Decisión:** la pantalla Tiendas ofrece botones por tipo de comercio ("verdulería cerca", "supermercado cerca"…) que abren la búsqueda en Google Maps, y "Cómo llegar" abre la ruta a pie. Sin API, sin clave y sin costo. Leaflet + OpenStreetMap queda como complementaria para la segunda entrega.
- **Alternativas:** `<iframe>` de OpenStreetMap (muestra el mapa pero no busca comercios); Leaflet desde la primera entrega.
- **Motivo:** cumple "buscar comercios cercanos y llegar caminando" sin librerías ni dependencias externas pagas.

## D-10 · Entorno inicial: solo local
- **Fecha:** 19/10/2026
- **Decisión:** el proyecto se levanta primero en local (app con `dotnet run` y PostgreSQL en Docker). El `Dockerfile` y el despliegue en Render quedan para más adelante.
- **Alternativas:** preparar el despliegue desde el inicio.
- **Motivo:** permite avanzar con el desarrollo sin depender de la cuenta de Render. Ojo: la base PostgreSQL gratis de Render vence a los 30 días, conviene crearla cerca de la entrega.
