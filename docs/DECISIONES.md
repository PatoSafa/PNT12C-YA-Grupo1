# Decisiones del equipo

Registro de decisiones técnicas y de alcance, con sus alternativas y el motivo. Es la "Evidencia 2 — Decisiones" que pide el enunciado (Parte 18).

Formato: **fecha · decisión · alternativas · motivo**.

---

## D-01 · Stack: C# / .NET con ASP.NET Core MVC
- **Fecha:** antes del 2026-10-26 (registrada en `CLAUDE.md`)
- **Decisión:** C# sobre .NET 10, ASP.NET Core MVC con vistas Razor, EF Core con migraciones y PostgreSQL en Docker. Bootstrap para estilos.
- **Alternativas:** Java + Spring Boot + MySQL (lo que usan las clases).
- **Motivo:** la cátedra habilitó .NET. ASP.NET Core MVC es el MVC de libro. PostgreSQL se usa igual en local y en Render, y su proveedor para EF Core (Npgsql) está bien mantenido.

## D-02 · Patrón: MVC con capa de Services y ViewModels
- **Fecha:** antes del 2026-10-26 (registrada en `CLAUDE.md`)
- **Decisión:** MVC, con Services para las reglas de negocio y ViewModels/DTOs hacia la vista.
- **Alternativas:** controllers que acceden directo a la base; API + frontend separado.
- **Motivo:** cada capa tiene una responsabilidad clara y es fácil de explicar en el oral (Clase 5 ya usa Service).

## D-03 · Resolución de conflictos de la Fase 0
- **Fecha:** 2026-10-26
- **Contexto:** al destilar las referencias aparecieron diferencias entre el enunciado, la documentación funcional y el prototipo de Figma. La documentación funcional es la más actualizada; el Figma quedó viejo.

| # | Conflicto | Decisión | Alternativa descartada |
|---|---|---|---|
| 1 | Alcance: el enunciado pide 1 entidad; la doc funcional, la app completa | **Se hace lo que está en la documentación funcional** | Entregar solo el mínimo del enunciado |
| 2 | El enunciado pide API JSON + `fetch`; el stack es MVC Razor | **Las dos cosas:** vistas Razor para la app y un controller JSON `/api/recetas` (GET, GET/{id}, POST, DELETE) con una página que lo use con `fetch` | Solo Razor (no cumple Partes 8–11 y 15) |
| 3 | El enunciado asume Spring + Gradle | **Se sigue con .NET** (`.sln` / `.csproj` en lugar de `build.gradle`) | Volver a Spring |
| 4 | El enunciado no tiene fecha | **Entrega: 2026-11-02** | — |
| 5 | Nombre: CenAPP / CenaAPP / CocinaFácil | **CenaAPP** | CocinaFácil, CenAPP |
| 6 | La doc funcional dice React/TS/Tailwind y "backend a definir" | **Backend C# con ASP.NET Core. Frontend lo más simple posible:** Razor + Bootstrap, JavaScript solo donde haga falta. Lo importante es que funcione | Frontend React |
| 7 | Faltan en el prototipo Registro, Inicio de sesión, Administración y "Cerrar sesión" | **Se arman siguiendo la estructura visual que ya tienen las demás pantallas** (header, tarjetas, botones) | Diseñarlas aparte en Figma |
| 8 | "Casi listas": ≥25 % (prototipo) vs ≥50 % (doc funcional) | **≥50 %** | ≥25 % |
| 9 | Alérgenos cargados a mano (con errores) vs calculados | **Se calculan a partir de los ingredientes** (RF8, VN-04) | Cargarlos a mano por receta |
| 10 | Listas de opciones distintas en Onboarding, Perfil y catálogos | **Todas salen de los mismos catálogos de la doc funcional** (16 restricciones, 13 alérgenos, 12 gustos) | Listas fijas por pantalla |
| 11 | Gustos: el prototipo no los usa | **Se usan para ordenar las recetas** (VN-08) | Ignorarlos |
| 12 | Favoritos solo en memoria vs guardados | **Se guardan en la base**, por usuario (RF5, RF12) | Guardarlos en el navegador |
| 13 | Tiendas: el prototipo muestra teléfono y rating | **Se sigue la doc funcional** (sin teléfono, sin precios) | Seguir el prototipo |
| 14 | Mapa: Leaflet real / simulado / rutas a pie | **Pospuesto.** Se resuelve más adelante | — |
| 15 | `CLAUDE.md` decía a la vez "solo referencia" y "usá de base ese código" | **El prototipo de Figma se usa solo como guía** (visual y de flujo). El código React no se reutiliza | Reutilizar el código React |

- **Motivo general:** la doc funcional es la fuente más completa y actualizada; el enunciado marca el piso que no puede faltar (API JSON, validaciones, manejo de ID inexistente, evidencias).
- **Riesgo registrado (resuelto en D-04):** el alcance completo con entrega el 2026-11-02 es mucho trabajo para el plazo. En la Fase 2 las tareas se ordenan para que primero quede cumplido todo lo que exige el enunciado y después se suma el resto de la doc funcional.

## D-04 · Alcance para la entrega del 2026-11-02
- **Fecha:** 2026-10-26
- **Contexto:** la documentación funcional completa (D-03) no entra en 7 días. Se recorta lo que cuesta mucho y aporta poco, sin perder el núcleo: perfil → recetas aptas → detalle → despensa.

**Entra**
| Bloque | Cómo |
|---|---|
| Requisitos del enunciado | `GET/GET{id}/POST/DELETE /api/recetas`, 404 claro, ≥2 validaciones propias (VAL-15, VAL-16), Docker, README, `ERRORES.md`, evidencias |
| Registro, login, logout, roles | Autenticación por cookie + `PasswordHasher`. El admin se crea en el seed |
| Onboarding y Perfil | Una misma vista para crear y editar: nombre, restricciones, alergias, gustos y nivel |
| Inicio | Recetas aptas (alérgenos y restricciones calculados desde los ingredientes), ordenadas por gustos, búsqueda por texto y filtro por gusto |
| Detalle | Ingredientes, pasos con checkbox (sin guardar), aviso de alérgenos |
| Favoritos | Marcar/desmarcar (con `fetch`) y **una lista "Mis favoritos"**. Nada más |
| Despensa | Agregar/quitar ingredientes del catálogo (busca por nombre o sinónimo), "Podés cocinar" (100 %) y "Casi listas" (≥ 50 %) |
| Tiendas | **Comercios hardcodeados** (unos pocos, cargados en el seed) con dirección, horario y "Cómo llegar". Sin servicio de mapas real |
| Admin | CRUD de ingredientes y de recetas |
| Imágenes | **Emojis**, sin fotos ni subida de archivos |
| Tests | xUnit sobre el cálculo de aptitud y el porcentaje de despensa |

**Datos iniciales (para poder simular)**
- **Restricciones y alergias: entre 5 y 10 en total** (no 16 + 13). Propuesta: 5 restricciones (Vegetariana, Vegana, Sin gluten, Sin lactosa, Baja en sodio) y 5 alérgenos (Gluten, Lácteos, Huevo, Frutos secos, Pescado y mariscos), tomados de la guía de restricciones del equipo.
- Gustos y categorías: los de la doc funcional (12 y 7).
- **~30 recetas** de `docs/funcional/guia_100_recetas_restricciones.pdf`, elegidas para cubrir categorías, gustos y restricciones, con los ingredientes que usan.
- Pasos, tiempo, dificultad y calorías no están en la guía: **los redacta Claude como borrador** y el equipo los revisa.

**Queda fuera de esta entrega**
- Servicio de mapas real, ubicación del usuario y rutas.
- Restricciones y alergias personalizadas (RF7, VN-07).
- Fotos de platos (VAL-20).
- El resto de las 118 recetas y 170 ingredientes (se pueden sumar después desde el admin o el seed).

- **Alternativas:** documentación completa (no entra en el plazo); solo el mínimo del enunciado (deja afuera lo que diferencia a la app).
- **Motivo:** se cumple todo el enunciado y el flujo principal de la app funciona de punta a punta, con datos suficientes para mostrarlo.

## D-05 · Acceso a datos: Services usan el `DbContext` directamente
- **Fecha:** 2026-10-26
- **Decisión:** los Services reciben `AppDbContext` por inyección de dependencias y consultan con `db.Recetas`, `db.Ingredientes`, etc. No hay una capa Repository propia.
- **Alternativas:** Repository por entidad encima de EF Core (`IRecetaRepository`…); mixto (Repository solo para `Receta`).
- **Motivo:** EF Core ya implementa el patrón: `DbSet<T>` cumple el rol de `JpaRepository` (Clase 3, Enunciado Parte 7) y `SaveChanges` confirma los cambios. Otra capa duplicaría código sin aportar, y el plazo es corto. En el oral: "¿qué problema nos evita?" → no escribir SQL a mano ni el CRUD básico de cada entidad.

## D-06 · Organización del código: carpetas por capa
- **Fecha:** 2026-10-26
- **Decisión:** estructura estándar de ASP.NET Core MVC: `Controllers/`, `Models/`, `ViewModels/`, `Services/`, `Data/`, `Views/{Controller}/`, `wwwroot/`.
- **Alternativas:** carpetas por módulo (`Recetas/`, `Despensa/`…); por capa con subcarpetas por módulo.
- **Motivo:** es la estructura por defecto del framework (Razor busca las vistas en `Views/{Controller}/` sin configurar nada) y la que se vio en clase (`controller/`, `model/`, `repository/`). Cada carpeta es una capa del recorrido View → Controller → Service → DbContext, fácil de explicar en el oral. Con ~6 controllers y ~5 services las carpetas no se saturan.

## D-07 · Pantallas: Razor + `fetch` solo en Favoritos y Despensa
- **Fecha:** 2026-10-26
- **Decisión:** todas las pantallas son vistas Razor con formularios clásicos (POST → el servidor procesa → recarga), salvo dos lugares con `fetch`:
  - **Favoritos:** el corazón marca/desmarca sin recargar.
  - **Despensa:** el formulario "Agregar ingrediente" hace `fetch`, y el servidor devuelve como vista parcial (HTML ya armado) el bloque "Podés cocinar / Casi listas", que el JavaScript reemplaza en la página. Cumple la Parte 15 del enunciado con una funcionalidad real.
- Los endpoints JSON `/api/recetas` (Partes 8–11) existen igual y se prueban con `.http` / Postman.
- **Alternativas:** todo Razor con una página de demo para la Parte 15; `fetch` en casi todas las pantallas.
- **Motivo:** el único lugar donde no recargar aporta de verdad es la Despensa (se agregan varios ingredientes seguidos). El JS queda corto porque el HTML lo sigue armando Razor, y el front se mantiene simple.

## D-08 · Modelo de datos: el de la doc funcional, simplificado con EF Core
- **Fecha:** 2026-10-26
- **Decisión:**
  - Entidades: `Usuario`, `Restriccion`, `Alergeno`, `Gusto`, `Categoria`, `Receta`, `Ingrediente`, `RecetaIngrediente`, `Paso`.
  - Relaciones muchos a muchos **sin clase intermedia** (EF Core crea la tabla): Usuario↔Restriccion, Usuario↔Alergeno, Usuario↔Gusto, Despensa (Usuario↔Ingrediente), Favoritos (Usuario↔Receta), Ingrediente↔Alergeno, Ingrediente↔Restriccion (incompatibles), Receta↔Gusto.
  - `RecetaIngrediente` es la única clase intermedia, porque guarda cantidad, unidad y nota.
  - `Paso` es una entidad (1:N con `Receta`, con orden).
  - Rol, nivel, dificultad y tipo de gusto son `enum` guardados como texto.
  - Los comercios son una lista fija en `ComercioService`, sin tabla.
  - Los alérgenos y las restricciones aptas de una receta **no se guardan: se calculan** desde sus ingredientes en `RecetaService`.
- **Alternativas:** una clase por cada tabla intermedia (~19 archivos, como la doc al pie de la letra); modelo mínimo con pasos y alérgenos como texto (rompe RF8).
- **Motivo:** respeta el modelo funcional con la mitad de archivos. Muestra los tres tipos de relación (N:1, 1:N, N:M con y sin datos propios), que responde el "desafío extra" del enunciado. Calcular alérgenos desde los ingredientes evita los errores que tenía el prototipo.

## D-09 · Estilos: Bootstrap + clases propias para los componentes de Figma
- **Fecha:** 2026-10-26
- **Decisión:** Bootstrap para la grilla, formularios y botones base. `wwwroot/css/estilos.css` define los colores de Figma como variables en `:root` (pisan los de Bootstrap) y unas 10 clases propias para lo característico del diseño (header con degradado por sección, tarjeta de receta, chips, barra inferior, alerta de alérgenos, barra de progreso). Ver `DISENO.md` §5.
- **Alternativas:** Bootstrap casi puro (se ve genérico); CSS propio sin Bootstrap (+1–2 días).
- **Motivo:** fiel al diseño con poco trabajo; los colores se cambian en un solo lugar.

## D-10 · Íconos: Bootstrap Icons (complementaria)
- **Fecha:** 2026-10-26
- **Decisión:** Bootstrap Icons, cargado con un `<link>` en `_Layout.cshtml`. Se usa como `<i class="bi bi-house"></i>`.
- **Alternativas:** emojis (cero dependencias, menos prolijo en la barra y botones); SVG inline (ensucia las vistas).
- **Cuatro filtros:** (1) resuelve los íconos de barra, botones y tarjetas sin dibujarlos a mano; (2) no reemplaza ninguna pieza de la base; (3) es una sola línea en el layout: si se saca, los íconos se cambian por emojis sin tocar nada más; (4) se explica en tres líneas y es de la misma familia que Bootstrap.

## D-11 · Mapa de Tiendas: Leaflet + OpenStreetMap (complementaria)
- **Fecha:** 2026-10-26
- **Decisión:** la vista de Tiendas muestra la lista de comercios fijos (de `ComercioService`) y un mapa dibujado con Leaflet sobre OpenStreetMap, con un marcador por comercio. Al tocar uno de la lista, el mapa se centra en él. "Cómo llegar" abre Google Maps / OpenStreetMap con la dirección. El código del mapa queda solo en `wwwroot/js/tiendas.js`.
- **Alternativas:** sin mapa (solo lista); `<iframe>` de OpenStreetMap (sin dependencias, pero un solo marcador a la vez).
- **Cuatro filtros:** (1) varios marcadores en un mapa no se logran con la base; (2) no reemplaza nada; (3) aislado en un JS y una vista; si se saca o falla, la lista con "Cómo llegar" sigue funcionando (RNF18, RNF22); (4) se explica en el oral: "Leaflet dibuja el mapa; los datos salen del servidor".

## D-12 · Integración con el trabajo del 19/10 (esqueleto y decisiones previas)
- **Fecha:** 2026-10-26
- **Contexto:** el 19/10 se subió en paralelo un esqueleto del proyecto (`CocinaFacil.sln`, `src/CocinaFacil.Web`, tests, `compose.yaml`, `dotnet-ef` como herramienta local) y un juego propio de decisiones (D-01 a D-10 de esa fecha). Las decisiones de este archivo (26/10) coinciden en casi todo; donde difieren, **mandan las del 26/10**:

| Tema | 19/10 | 26/10 (vigente) |
|---|---|---|
| Nombre | CocinaFacil | **CenaApp** (D-03) |
| Mapa | Links a Google Maps; Leaflet en la 2ª entrega | **Leaflet con comercios fijos** (D-11) |
| Admin de ingredientes | 2ª entrega | **Entra** (D-04) |
| Dockerfile y Render | Más adelante | **Antes de la entrega** (PLAN T20). Ojo: la base gratis de Render vence a los 30 días; crearla cerca de la entrega |

- **Se conserva** el esqueleto del 19/10: se renombra a `CenaApp` y cubre las tareas T01 y parte de T02 del plan.
- **Siguen pendientes** (planteadas el 19/10): idioma del código y si se mantiene jQuery.
- **Motivo:** las decisiones del 26/10 incorporan la documentación funcional final y el recorte de alcance por plazo.

## D-13 · Idioma del código: español
- **Fecha:** 2026-10-26
- **Decisión:** clases, propiedades, métodos y variables propias en español (`Receta`, `ObtenerAptas()`, `tiempoMin`). Lo que es del framework queda como viene (`Controller`, `View`, `DbContext`, `IActionResult`).
- **Alternativas:** inglés (`Recipe`, `GetSuitable()`).
- **Motivo:** coincide con la documentación funcional, las vistas y las decisiones; el equipo lo explica con las mismas palabras que el dominio.

## D-14 · jQuery: se mantiene solo para la validación del cliente
- **Fecha:** 2026-10-26
- **Decisión:** se conservan jQuery y jQuery Validation Unobtrusive que trae la plantilla MVC, solo para que las Data Annotations (`[Required]`, `[Range]`…) también se validen en el navegador. El JavaScript propio (favoritos, despensa, mapa) usa `fetch` nativo, no jQuery.
- **Alternativas:** quitarlo y validar solo en el servidor.
- **Motivo:** la validación en el navegador sale gratis (sin escribir JS) y el usuario ve el error antes de enviar el formulario. La validación del servidor (`ModelState.IsValid`) se mantiene igual: es la que garantiza los datos.

## D-15 · Zona de los comercios fijos: Almagro (ORT Yatay)
- **Fecha:** 2026-10-26
- **Decisión:** los ~6 comercios fijos de `ComercioService` (D-11) se ubican alrededor de la sede de ORT en Almagro, y el mapa se centra ahí. Son datos de ejemplo para la demo: nombres ficticios en calles reales de la zona.
- **Alternativas:** Belgrano (ORT Belgrano); centro (como el prototipo).
- **Motivo:** zona conocida por todo el equipo y el docente, la demo resulta natural.
