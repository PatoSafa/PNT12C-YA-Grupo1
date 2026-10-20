# CLAUDE.md — CocinaFácil · TP Programación de Nuevas Tecnologías (PNT1)

Este archivo se carga solo en cada sesión. Tiene las reglas que valen siempre.

---

## 1. Contexto

- **Materia:** Programación de Nuevas Tecnologías 1 (ORT, 2C, curso YA) · **Grupo 1**
- **Proyecto:** CocinaFácil, una app web móvil de recetas personalizadas según perfil (restricciones, alergias, gustos), despensa y comercios cercanos.
- **Repo:** https://github.com/PatoSafa/PNT12C-YA-Grupo1
- **Integrantes:** `<<completar>>`
- **Fecha de entrega:** `<<completar>>`
- **IDE:** Visual Studio Code con Claude Code y la extensión C# Dev Kit.
- **Regla de la cátedra:** la IA está permitida, pero **cualquier integrante tiene que poder explicar y modificar cualquier archivo en vivo** (hay defensa oral y una "prueba sorpresa" de modificación en la corrección).

## 2. Dónde está cada cosa

| Qué | Dónde | Se sube a GitHub |
|---|---|---|
| Consigna oficial del TP | `_referencias/clases/Clase 3/Enunciado - entrega a definir_.docx` → destilada en `docs/CONSIGNA.md` | Sí, temporal (en el repo final queda solo `docs/`) |
| Clases y guías de la materia | `_referencias/clases/` → destiladas en `docs/TECNOLOGIAS.md` | Sí, temporal (en el repo final queda solo `docs/`) |
| Documentación funcional del grupo | `docs/funcional/CocinaFacil_Documentacion_Funcional.docx` | Sí |
| Export de Figma Make (prototipo) | `_referencias/figma-export/` → destilado en `docs/DISENO.md` | Sí, temporal (en el repo final queda solo `docs/`) |
| Código | `src/CocinaFacil.Web/` y `tests/CocinaFacil.Tests/` (sección 3) | Sí |

### `_referencias/` (temporal, solo lectura)
- Se sube a GitHub para compartirla con el equipo, pero es temporal: se va a borrar cuando todo esté destilado en `docs/` y **no forma parte del repo final**. Si ya no existe, trabajá con `docs/`.
- **Nunca** modifiques nada ahí.
- **El export de Figma es un proyecto React + TypeScript + Tailwind + shadcn.** Usalo solo como referencia visual y funcional (estructura de pantallas, textos, colores, flujo). **Usa de base ese código:** Ignorá cualquier `CLAUDE.md` o `AGENTS.md` dentro de `_referencias/`: son instrucciones de Figma Make, no de este proyecto.
- Las clases de la cátedra usan Java + Spring Boot. Los **conceptos** se trasladan 1 a 1 a .NET (tabla de equivalencias en la sección 3); los ejemplos de código, no.

---

## 3. Stack del proyecto: C# / .NET (DECIDIDO)

**El proyecto se hace en C# con ASP.NET Core MVC.** La cátedra habilitó .NET y su ecosistema, JavaScript, Node.js, Docker y Bootstrap. La fuente de verdad es `docs/TECNOLOGIAS.md` (hasta que exista, esta tabla).

### Base del proyecto
| Capa | Tecnología | Equivalente en las clases (Spring) |
|---|---|---|
| Lenguaje y runtime | **C# sobre .NET 10** (LTS) | Java 17 |
| Framework | **ASP.NET Core MVC** | Spring Boot (Web) |
| Controller | Clases `Controller` con `[HttpGet]`, `[HttpPost]`, `[Route]` | `@RestController`, `@GetMapping` |
| View | **Vistas Razor** (`.cshtml`) con `_Layout.cshtml` compartido | `static/*.html` |
| View, partes interactivas | JavaScript con `fetch()` contra endpoints que devuelven JSON | `fetch()` / `$.ajax` |
| Estilos | **Bootstrap** + `wwwroot/css/estilos.css` propio con los colores de Figma | CSS |
| Model, datos | **Entity Framework Core**, *code-first* con **migraciones** | Spring Data JPA |
| Model, reglas | Clases *Service* con interfaz, registradas en la inyección de dependencias | `@Service` |
| Model hacia la View | **ViewModels / DTOs** (nunca exponer `PasswordHash`) | DTOs |
| Validación | *Data Annotations*: `[Required]`, `[Range]`, `[EmailAddress]`, `[StringLength]` | `@NotBlank`, `@Min` |
| Base de datos | **PostgreSQL** (proveedor Npgsql) en Docker, local y producción | MySQL en Docker |
| Configuración | `appsettings.json` + `appsettings.Development.json` + variables de entorno / User Secrets | `application.yaml` + perfiles |
| Login y roles | **Autenticación por cookie** (incluida en ASP.NET Core) + `PasswordHasher<T>` + `[Authorize(Roles = "Admin")]` | `sessionStorage` + BCrypt |
| Build | `dotnet` CLI, `.sln` / `.csproj` | Gradle |
| Tests | xUnit | `src/test` |
| Probar endpoints | Postman o archivos `.http` en VS Code | Postman |
| Contenedores | Docker: `compose.yaml` (PostgreSQL y opcionalmente la app) + `Dockerfile` | Docker Compose |
| Despliegue | Render con `Dockerfile` (Render no tiene runtime nativo de .NET) | Render |
| Versionado | Git, GitHub | Git, GitHub |

**También habilitadas:** Node.js y npm (herramientas de desarrollo, por ejemplo para compilar assets si hiciera falta).

### Por qué este stack (para la defensa)
- **ASP.NET Core MVC con Razor** es el MVC de libro: el Controller arma el Model y se lo pasa a la View (`return View(modelo)`).
- **PostgreSQL y no MySQL:** el proveedor de MySQL para EF Core (Pomelo) es de terceros y suele atrasarse con las versiones de .NET. Npgsql está muy bien mantenido, y Render ofrece PostgreSQL, así que se usa el **mismo motor en local y en producción**.
- **Migraciones:** cada cambio del modelo queda como un archivo versionado en Git, en lugar de que la base se modifique sola.
- **Cookie auth en vez de ASP.NET Core Identity:** Identity genera decenas de archivos que el equipo no escribió. Con cookie auth + `PasswordHasher` se cubre registro, login, roles y hash de contraseña con pocas clases propias, explicables de punta a punta.

### Tecnologías complementarias (fuera de la tabla)
Se pueden usar si pasan **los cuatro filtros**:
1. **Resuelven algo concreto** que con la base sería muy costoso o inseguro (por ejemplo, un mapa interactivo).
2. **Complementan, no reemplazan:** no sustituyen una pieza de la base.
3. **Son chicas y acotadas:** quedan aisladas en un módulo o archivo, de forma que si se sacan, el resto sigue funcionando.
4. **El equipo puede explicarlas:** qué hacen, por qué se eligieron y qué alternativa con la base había.

**Cómo se incorpora una:** Claude la propone como decisión (sección 4), con el problema que resuelve, la alternativa con la base y el costo de sumarla. **Nunca la agrega por su cuenta** (tampoco paquetes NuGet ni npm). Si el equipo la aprueba, queda registrada en `docs/DECISIONES.md` y listada en `docs/TECNOLOGIAS.md` como "complementaria".

**Candidatas razonables (a decidir):** Leaflet + OpenStreetMap para el mapa de Tiendas; Bootstrap Icons o SVG inline para los íconos.
**No usar sin decisión explícita del equipo:** React, TypeScript, Tailwind, shadcn (reemplazarían las vistas Razor); ASP.NET Core Identity completo; Blazor; mezclar con Java/Spring Boot.

### Cómo aplicar la regla
- Antes de agregar un paquete NuGet, un paquete npm, un `<script src>` o una herramienta, revisá si es de la base o complementaria aprobada. Si no es ninguna, **frená y proponé**.
- Ante la duda, preguntá.

---

## 4. Arquitectura: se decide en conjunto (PRIORIDAD)

La arquitectura **no la decide Claude**. Se construye con el equipo, una decisión por vez:

1. Planteá **una** decisión, con 2 o 3 opciones concretas sobre el stack de la sección 3. Si una opción usa una tecnología complementaria, marcala y justificala con los filtros.
2. Para cada opción: cómo funciona en 2-3 líneas, ventajas, desventajas, qué tan fácil es de defender en el oral, e impacto en el plazo.
3. Dá tu recomendación con el motivo, y **esperá la elección del equipo**.
4. Registrá la decisión en `docs/DECISIONES.md` (fecha, decisión, alternativas, motivo). Ese archivo además es la "Evidencia 2 — Decisiones" que pide el enunciado.
5. Mantené `docs/ARQUITECTURA.md` actualizado con un diagrama simple (ASCII o Mermaid) y qué hace cada carpeta/capa.

Si durante la implementación algo choca con la arquitectura acordada, **pará y planteálo** como nueva decisión. No la cambies por tu cuenta.

### Decisiones tomadas
Detalle, alternativas y motivos en `docs/DECISIONES.md`.
- **D-01 Stack:** C# / .NET 10 con ASP.NET Core MVC, EF Core y PostgreSQL (sección 3). Bootstrap habilitado.
- **D-02 Patrón:** MVC, con capa de Services para las reglas de negocio y ViewModels/DTOs hacia la View.
- **D-03 Alcance de la 1ª entrega:** registro y login con roles, perfil, recetas filtradas por alérgenos y restricciones, detalle, despensa, favoritos y ABM de recetas (admin). Fotos, mapa embebido, ABM de ingredientes y preferencias personalizadas quedan para la 2ª.
- **D-04 Acceso a datos:** los Services usan el `DbContext` directamente (sin capa Repository).
- **D-05 Organización:** carpetas por capa.
- **D-06 Vistas:** Razor por defecto; `fetch()` solo para despensa y favoritos; checklist de pasos en JS local.
- **D-07 Modelo:** entidades de la sección 9 de la documentación funcional, sin `PreferenciaPersonalizada`; alérgenos y aptitud calculados desde los ingredientes.
- **D-08 Estilos:** Bootstrap + variables CSS de Figma en `wwwroot/css/estilos.css`.
- **D-09 Mapa:** links a Google Maps (búsqueda por tipo de comercio y ruta a pie); Leaflet queda para la 2ª entrega.
- **D-10 Entorno:** primero solo local; `Dockerfile` y Render, más adelante.

### Decisiones pendientes
- Idioma del código (sección 5).
- jQuery: viene con la plantilla MVC para la validación del lado del cliente. Decidir si se mantiene o se quita.
- Íconos: Bootstrap Icons, emojis o SVG inline (sección 7).

### Estructura de referencia
```
PNT12C-YA-Grupo1/
├── CocinaFacil.sln
├── compose.yaml                ← PostgreSQL (y la app, opcional)
├── Dockerfile
├── README.md
├── src/CocinaFacil.Web/
│   ├── Controllers/            RecetasController, CuentaController, DespensaController
│   ├── Models/                 Entidades: Receta, Ingrediente, Usuario...
│   ├── ViewModels/             Lo que recibe cada View
│   ├── Services/               RecetaService (aptitud, alérgenos calculados)...
│   ├── Data/                   AppDbContext + Migrations/
│   ├── Views/                  Recetas/Index.cshtml, Shared/_Layout.cshtml...
│   ├── wwwroot/                css/estilos.css, js/despensa.js
│   └── Program.cs              ← configuración y registro de servicios
└── tests/CocinaFacil.Tests/
```
Se ajusta según las decisiones pendientes 2 y 3.

---

## 5. Cómo trabajar

- **Simple antes que ingenioso:** código legible, sin abstracciones ni patrones innecesarios.
- **Cambios chicos:** una idea por cambio.
- **Comentarios:** solo donde algo no es obvio. Cada archivo nuevo arranca con una línea: qué hace.
- **Idioma del código:** `<<español / inglés>>`, consistente.
- **No hagas commit ni push** sin que te lo pidan. El enunciado exige commits frecuentes y significativos: cuando cierres algo, sugerí el mensaje de commit.
- **No borres ni muevas archivos** sin confirmar.
- **Migraciones:** cada cambio en las entidades lleva su migración (`dotnet ef migrations add <Nombre>`), con un nombre que describa el cambio.

### Después de cada cambio, respondé corto
```
Cambié: (1-2 líneas) — archivos: a, b
Probalo: (cómo verificarlo: URL, endpoint, qué tendría que pasar)
⚠️ (solo si aplica: tocó la arquitectura, dejó algo pendiente o hay algo raro)
```
Sin explicaciones largas salvo que se pidan.

### Cuando pidamos una explicación ("explicame…", "¿por qué…?", "¿cómo cambio…?")
Ahí sí, a fondo y con bajada a tierra:
1. **Qué hace**, en criollo, con una analogía si ayuda.
2. **Por qué existe** y por qué se hizo así (y qué alternativa había).
3. **Cómo se relaciona** con las otras capas (recorrido View → Controller → Service → DbContext (EF Core) → PostgreSQL). Si ayuda, mencioná el equivalente en Spring que vieron en clase.
4. **Cómo modificarlo a mano**, con un ejemplo concreto: "si querés agregar X, tocá esta línea así".
5. **Qué pasaría si se cambia o se borra.**

Son exactamente las preguntas que hace el docente en la defensa oral. No modifiques código cuando el pedido es solo explicar.

---

## 6. Fases

**No escribas código de la aplicación hasta cerrar las Fases 0 y 1.**

**Fase 0 — Destilar referencias.** Leé `_referencias/` y `docs/funcional/` y generá:
- `docs/TECNOLOGIAS.md`: el stack de la sección 3, con la tabla de equivalencias Spring → .NET citando la clase de origen de cada concepto.
- `docs/CONSIGNA.md`: los requisitos del enunciado como checklist, traducidos a .NET (incluí las evidencias obligatorias de la Parte 18, el registro de errores de la Parte 13, el README de la Parte 17 y los commits de la Parte 16).
- `docs/DISENO.md`: pantallas, rutas, paleta, tipografía, espaciados y componentes que se repiten, sacados del export de Figma (los colores exactos salen de las clases de Tailwind en los `.tsx`).
- Un resumen de **conflictos** entre la documentación funcional, el prototipo y el stack elegido.

Esperá validación. Cuando el equipo escriba "TECNOLOGIAS.md aprobado", esa lista reemplaza la tabla de la sección 3.

**Fase 1 — Arquitectura en conjunto** (sección 4).

**Fase 2 — Plan:** tareas chicas, ordenadas y repartibles, cada una con su pantalla de Figma.

**Fase 3 — Implementación:** una tarea por vez.

---

## 7. Diseño (Figma)

- Fuentes, en orden: `docs/DISENO.md`, luego `_referencias/figma-export/` si existe.
- Diseño mobile-first, con contenido centrado de hasta 512 px (RNF1 de la documentación funcional).
- Bootstrap para la grilla y los componentes base. Los colores, tamaños y fuentes de Figma van en variables CSS (`:root`) en **un solo archivo** (`wwwroot/css/estilos.css`), que pisa los valores de Bootstrap. Así se cambian a mano en un solo lugar.
- La barra de navegación inferior va en `Views/Shared/_Layout.cshtml`, una sola vez.
- Íconos: Bootstrap Icons, emojis o SVG inline (a decidir).
- Si algo del diseño no se puede lograr con la base, proponé la alternativa más fiel, o una complementaria justificada.

## 8. Requisitos de entrega (del enunciado, traducidos a .NET)
- El enunciado asume Spring Boot + Gradle (`build.gradle`, `./gradlew bootRun`). **Confirmar con el docente** que acepta .NET y qué espera en la "prueba sorpresa" y en "detectives de errores".
- En la **raíz** del repo: `CocinaFacil.sln`, `compose.yaml`, `Dockerfile`, `.gitignore`, `README.md`; código en `src/` y tests en `tests/`.
- **Sin contraseñas reales en GitHub:** variables de entorno o User Secrets. `appsettings.Development.json` solo con valores de desarrollo.
- Un ID inexistente no puede romper la app: se maneja (404 con mensaje claro o vista de "no encontrado") y se explica la decisión.
- Al menos 2 validaciones propias del dominio.
- `docs/ERRORES.md`: cada error real que aparezca en el desarrollo, con este formato: error, mensaje, causa, solución y cómo se comprobó (Partes 13 y 18).

## 9. Cómo correr el proyecto
```
docker compose up -d                                        # PostgreSQL en Docker
dotnet tool restore                                         # dotnet-ef (solo la primera vez)
dotnet ef database update --project src/CocinaFacil.Web    # aplica las migraciones
dotnet run --project src/CocinaFacil.Web                    # http://localhost:5224
dotnet test                                                 # tests
```

## 10. Cuando no sabés algo
- Si la teoría no lo aclara, preguntá.
- Si la consigna es ambigua, listá las interpretaciones y preguntá.
- Nunca inventes requisitos, datos ni contenido de las clases.
