# Plan de tareas (Fase 2)

Tareas chicas y ordenadas para llegar a la entrega del **lunes 2 de noviembre de 2026**. Las etapas 1 y 6 cubren todo lo que exige el enunciado; el resto es la app según D-04.

> T01 quedó cubierta por el esqueleto del 19/10, renombrado a CenaApp (D-12).

**Cómo usarlo**
- Cada tarea es un cambio chico que termina en **un commit** (el enunciado pide ≥ 4 significativos; acá hay ~22).
- Completen la columna **Resp.** con quién la toma. Las tareas sin dependencia entre sí se pueden hacer en paralelo.
- "Diseño" indica la sección de `DISENO.md` a mirar.
- Al terminar una tarea: marcarla `[x]`, y si apareció un error, anotarlo en `docs/ERRORES.md`.

## Etapa 0 · Entorno (lunes 26) — cada integrante

| ✓ | # | Tarea | Resp. |
|---|---|---|---|
| [ ] | T00 | Instalar **.NET 10 SDK**, **Docker Desktop** y la herramienta de migraciones (`dotnet tool install -g dotnet-ef`). Verificar con `dotnet --version`, `docker --version`, `dotnet ef --version` | todos |

## Etapa 1 · Base del proyecto (lunes 26 – martes 27) — cubre Partes 1–12 del enunciado

| ✓ | # | Tarea | Depende | Diseño | Commit sugerido | Resp. |
|---|---|---|---|---|---|---|
| [x] | T01 | Crear `CenaApp.sln`, `src/CenaApp.Web` (`dotnet new mvc`), `tests/CenaApp.Tests` (xUnit) y `.gitignore` de .NET | T00 | — | `chore: inicializar solución ASP.NET Core MVC` | |
| [ ] | T02 | ~~`compose.yaml` con PostgreSQL + volumen; cadena de conexión en `appsettings.Development.json`~~ (hecho en el esqueleto del 19/10). Levantado y conectado el 26/10 (ver E-01 en `ERRORES.md`). **Falta:** la captura del contenedor corriendo (Parte 4) | T01 | — | `chore: PostgreSQL con Docker Compose` | |
| [x] | T03 | Diseño de la entidad `Receta` antes de programar: tabla atributo/tipo + justificación (Parte 3, Evidencia 1) en `docs/DISENO_ENTIDADES.md` | — | — | `docs: diseño de entidades` | |
| [x] | T04 | Entidades (`Models/`), enums, `AppDbContext` con las relaciones (D-08) y migración `Inicial` | T02, T03 | — | `feat: entidades, DbContext y migración inicial` | |
| [x] | T05 | Datos iniciales: 5 restricciones, 5 alérgenos, 12 gustos, 7 categorías, ingredientes, ~30 recetas con pasos, usuario admin. Claude redacta el borrador; **el equipo lo revisa** (pendiente: tiempos, dificultad, calorías y pasos en `Data/DatosIniciales.cs`) | T04 | — | `feat: datos iniciales` | |
| [x] | T06 | `RecetaService` (listar, buscar, crear, eliminar) + `Api/RecetasApiController` (`GET`, `GET/{id}`, `POST`, `DELETE`) con DTO, validaciones VAL-15/VAL-16 y 404 con mensaje. Archivo `recetas.http` para probar (Partes 7–12) | T05 | — | `feat: API REST de recetas` | |
| [x] | T07 | `README.md` inicial (cómo correr) y `docs/ERRORES.md` con el formato de la Parte 13 | T06 | — | `docs: README y registro de errores` | |

## Etapa 2 · Layout y cuentas (miércoles 28)

| ✓ | # | Tarea | Depende | Diseño | Commit sugerido | Resp. |
|---|---|---|---|---|---|---|
| [x] | T08 | `_Layout.cshtml` con barra inferior, `estilos.css` con variables de color y clases propias, Bootstrap Icons | T01 | §1.1, §2, §5 | `feat: layout, barra inferior y estilos base` | |
| [x] | T09 | Registro, inicio y cierre de sesión con cookie + `PasswordHasher`; roles Usuario/Admin; `[Authorize]` | T04, T08 | §1.2 (misma estructura) | `feat: registro, login y roles` | |
| [x] | T10 | Onboarding y Perfil (misma vista para crear y editar): nombre, restricciones, alergias, gustos, nivel. Botón "Cerrar sesión" | T09 | §1.2, §1.7 | `feat: onboarding y perfil` | |

## Etapa 3 · Recetas (jueves 29)

| ✓ | # | Tarea | Depende | Diseño | Commit sugerido | Resp. |
|---|---|---|---|---|---|---|
| [x] | T11 | Cálculo de aptitud en `RecetaService`: alérgenos desde ingredientes, restricciones aptas, orden por gustos. **Tests xUnit** | T05 | — | `feat: cálculo de recetas aptas + tests` | |
| [x] | T12 | Inicio: saludo, buscador, filtro por gusto, tarjetas (vista parcial `_TarjetaReceta`), estado vacío | T10, T11 | §1.3 | `feat: pantalla de inicio` | |
| [x] | T13 | Detalle: datos, aviso de alérgenos, pestañas, pasos con checkbox (JS), vista "no encontrada" con 404 | T11 | §1.4 | `feat: detalle de receta` | |
| [x] | T14 | Favoritos: corazón con `fetch` + lista "Mis favoritos" | T12 | §1.3 | `feat: favoritos` | |

## Etapa 4 · Despensa y Tiendas (viernes 30)

| ✓ | # | Tarea | Depende | Diseño | Commit sugerido | Resp. |
|---|---|---|---|---|---|---|
| [x] | T15 | `DespensaService`: agregar/quitar/vaciar, porcentaje por receta, "Podés cocinar" / "Casi listas" (≥ 50 %). **Tests xUnit** | T11 | — | `feat: lógica de despensa + tests` | |
| [ ] | T16 | Vista Despensa: formulario con `fetch` que reemplaza la vista parcial de resultados (Parte 15) | T15, T08 | §1.5 | `feat: pantalla de despensa con AJAX` | |
| [ ] | T17 | Tiendas: `ComercioService` con ~6 comercios fijos, lista con filtro por tipo, mapa Leaflet, "Cómo llegar" | T08 | §1.6 | `feat: tiendas con mapa` | |

## Etapa 5 · Administración (sábado 31)

| ✓ | # | Tarea | Depende | Diseño | Commit sugerido | Resp. |
|---|---|---|---|---|---|---|
| [ ] | T18 | Admin de ingredientes: listado, alta, edición, baja (no se borra si está en uso, VN-21) | T09 | estructura de §5 | `feat: admin de ingredientes` | |
| [ ] | T19 | Admin de recetas: listado, alta/edición con ingredientes en filas y pasos en textarea, baja con confirmación | T18, T11 | estructura de §5 | `feat: admin de recetas` | |

## Etapa 6 · Entrega (domingo 1)

| ✓ | # | Tarea | Depende | Commit sugerido | Resp. |
|---|---|---|---|---|---|
| [ ] | T20 | `Dockerfile` + deploy en Render con PostgreSQL de Render (variables de entorno, sin secretos en Git) | T06 | `chore: Dockerfile y despliegue en Render` | |
| [ ] | T21 | README completo (Parte 17): descripción, tecnologías, cómo correr, endpoints, JSON de ejemplo | T20 | `docs: README completo` | |
| [ ] | T22 | Evidencias: capturas, `ERRORES.md` con al menos un error real, reflexión (Parte 20), repasar `CONSIGNA.md` ítem por ítem | todas | `docs: evidencias y reflexión final` | |
| [ ] | T23 | Ensayo de defensa: cada integrante explica un recorrido completo (View → Controller → Service → DbContext → PostgreSQL) y hace un simulacro de prueba sorpresa (Parte 19) | todas | — | todos |

## Camino crítico
`T00 → T01 → T02 → T04 → T05 → T11 → T12/T13/T15 → T19 → T22`

En paralelo desde el inicio: T03 (diseño de entidades) y, después de T01, T08 (layout y estilos). Si el tiempo aprieta, lo primero que se recorta es T19 (admin de recetas: se puede dejar solo alta y baja) y T17 (dejar la lista sin mapa).
