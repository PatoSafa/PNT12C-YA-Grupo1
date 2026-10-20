# Consigna del TP — checklist

Fuente: `_referencias/clases/Clase 3/Enunciado - entrega a definir_.docx` ("Práctica integradora"). El enunciado está escrito para Java + Spring Boot + Gradle + MySQL; acá cada requisito está traducido a nuestro stack (.NET, ver `docs/TECNOLOGIAS.md`).

> **Pendiente con el docente:** confirmar que acepta .NET en lugar de Spring Boot, PostgreSQL en lugar de MySQL y `.sln` / `.csproj` en lugar de `build.gradle` / `settings.gradle`, y qué espera en "Detectives de errores" (Parte 13) y en la "Prueba sorpresa" (Parte 19).

Estado: ✅ hecho · 🟡 en curso · ⬜ pendiente

## Construcción

| # | Parte del enunciado | Qué pide | Cómo se cumple en .NET | Estado |
|---|---|---|---|---|
| 1 | Preparación del proyecto | Proyecto con Spring Web, Data JPA, driver MySQL y Validation; nombre ligado al dominio | Solución `CocinaFacil.sln` con ASP.NET Core MVC, EF Core, Npgsql y Data Annotations | ✅ |
| 2 | Elegir un dominio | Un dominio que guarde datos ("Recetas" está en la lista) | Recetas (CocinaFácil) | ✅ |
| 3 | Diseñar antes de programar | Entidad, atributos, tipo de cada atributo y justificación, **antes de programar** | Tabla de diseño de `Receta` (y relacionadas) con tipos C#, en `docs/` antes de crear las clases. Es la **Evidencia 1** | ⬜ |
| 4 | Base de datos con Docker | `compose.yaml` con imagen oficial, base, usuario, contraseña, puerto y volumen; `docker compose up -d` / `down`; **captura** del contenedor corriendo | `compose.yaml` con `postgres:17` (base `cocinafacil`, volumen `cocinafacil-datos`). Falta la captura | 🟡 |
| 5 | Configuración | Conectar la app a la base con JPA; **sin contraseñas reales en GitHub** | `AppDbContext` + `UseNpgsql`; cadena de desarrollo en `appsettings.Development.json`, producción por variable de entorno | ✅ |
| 6 | Crear la entidad | `@Entity`, `@Id`, id autogenerado, atributos del diseño; explicar por qué es entidad, cuál es el id y su relación con la tabla | Clase en `Models/` con `Id` (clave autogenerada por convención de EF Core), `DbSet<Receta>` y migración que crea la tabla | ⬜ |
| 7 | Repository | Obtener todos, por id, guardar y eliminar sin escribirlos a mano; explicar qué evita `JpaRepository` | `DbSet<T>`: `ToListAsync`, `FindAsync`, `Add` + `SaveChangesAsync`, `Remove` (decisión D-04) | ⬜ |
| 8 | API REST | Mínimo `GET /api/recurso`, `GET /api/recurso/{id}`, `POST /api/recurso`, `DELETE /api/recurso/{id}` | Controller con `[ApiController]` y `[Route("api/recetas")]` (ver conflicto C-01 en `docs/CONFLICTOS.md`) | ⬜ |
| 9 | Crear registros | `POST` que recibe JSON y lo guarda | `[HttpPost]` con `[FromBody]` y DTO de entrada | ⬜ |
| 10 | Consultar registros | `GET` de todos y por id; **decidir y explicar qué pasa con un id inexistente** | `[HttpGet]` y `[HttpGet("{id}")]`; id inexistente → `404 NotFound` con mensaje claro (y vista "no encontrado" en las pantallas Razor) | ⬜ |
| 11 | Eliminar | `DELETE`: comprobar que existe, que se elimina y que después no aparece en el `GET` | `[HttpDelete("{id}")]`: 404 si no existe, 204 si se borró | ⬜ |
| 12 | Validación | Validaciones sobre los datos recibidos; **al menos 2 reglas propias del dominio**, explicadas | Data Annotations (`[Required]`, `[Range]`, `[StringLength]`, `[EmailAddress]`) + reglas de negocio en Services (por ejemplo, receta con al menos un ingrediente) | ⬜ |
| 15 | Frontend / AJAX | Página con formulario, inputs, botón y JavaScript con `fetch()` que muestre la respuesta | Despensa y favoritos con `fetch()` (D-06), más el formulario de alta que llame al `POST` de la API | ⬜ |
| 16 | Git y GitHub | Repo con README, build, `compose.yaml`, `src/`; **al menos 4 commits significativos** durante el desarrollo | Commits por tarea, con mensajes que describan lo hecho | 🟡 |
| 17 | README | Nombre, descripción, tecnologías, cómo ejecutar, **endpoints** y **un ejemplo de JSON** | `README.md` ya tiene nombre, descripción, tecnologías y cómo ejecutar. Faltan endpoints y JSON | 🟡 |

## Corrección y defensa

| # | Parte | Qué pide | Cómo prepararse | Estado |
|---|---|---|---|---|
| 13 | Detectives de errores | Recibir un proyecto con errores (código, controller, repository, configuración de base, Docker, build, endpoints), encontrarlos y corregirlos. **Registro obligatorio** por error: error, mensaje, causa, solución y cómo se comprobó | `docs/ERRORES.md` con ese formato, completado con errores reales del desarrollo. Practicar leyendo errores de `dotnet build`, EF Core y Docker | ⬜ |
| 14 | Cambio de requisitos | El docente asigna uno: A) nuevo atributo, B) nuevo endpoint, C) búsqueda (`GET /api/recetas?nombre=...`), D) segunda entidad, E) nueva validación, F) cambiar un endpoint. Sin código de solución | Cada integrante tiene que saber hacer A–F solo. Atributo nuevo = propiedad + migración + DTO + vista | ⬜ |
| 18 | Evidencias de intervención humana | 1) Diseño de la entidad · 2) **Tres decisiones técnicas** · 3) Un error real y su solución · 4) Qué se cambió por el nuevo requisito · 5) Los commits · 6) Explicación oral de cualquier archivo | 1) Parte 3 · 2) `docs/DECISIONES.md` · 3) `docs/ERRORES.md` · 4) Sección en `docs/` cuando ocurra · 5) Historial de Git · 6) Regla de la cátedra (`CLAUDE.md`, sección 1) | 🟡 |
| 19 | Prueba sorpresa | Modificación en vivo: endpoint de búsqueda por palabra, atributo nuevo en POST y GET, o cambiar la respuesta de id inexistente | Mismo entrenamiento que la Parte 14 | ⬜ |
| 20 | Reflexión final | Responder 7 preguntas: lo más fácil, lo más difícil, el error que más enseñó, MySQL directo vs Docker, **qué función cumple Gradle**, recorrido de una petición desde el Controller hasta la base, qué harían distinto | Traducir: Gradle → `dotnet` CLI y `.csproj`; recorrido Controller → Service → `DbContext` → PostgreSQL | ⬜ |
| — | Desafío extra | Segunda entidad con relación (JPA), endpoints, datos de prueba, README actualizado; explicar el tipo de relación y cómo se ve en la base | Ya está en el modelo (Receta–Ingrediente, Receta–Paso). Saber explicar cómo EF Core crea las claves foráneas y la tabla intermedia | ⬜ |

## Entrega final

El enunciado pide el enlace al repositorio de GitHub con esta estructura en la raíz (traducida):

| Enunciado (Spring) | Nuestro repo (.NET) | Estado |
|---|---|---|
| `README.md` | `README.md` | 🟡 |
| `build.gradle`, `settings.gradle` | `CocinaFacil.sln`, `src/CocinaFacil.Web/CocinaFacil.Web.csproj` | ✅ |
| `compose.yaml` | `compose.yaml` | ✅ |
| `.gitignore` | `.gitignore` | ✅ |
| `src/main/java/...`, `src/main/resources/...` | `src/CocinaFacil.Web/` | ✅ |
| `src/test/` | `tests/CocinaFacil.Tests/` | ✅ |
| Documentación de decisiones y evidencias | `docs/` | 🟡 |

Además, antes de la entrega final se borra `_referencias/` del repo.

## Criterio de evaluación (textual del enunciado)

"El proyecto debe funcionar, pero que funcione no es suficiente." Se evalúa que cada estudiante pueda explicar su código, detectar errores, justificar decisiones, modificar el proyecto, interpretar mensajes de error, hacer cambios sin recibir la solución, mostrar el proceso con Git y responder preguntas de arquitectura.
