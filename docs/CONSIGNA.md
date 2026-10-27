# Consigna — checklist

Requisitos del enunciado "Práctica integradora — Java + Spring Boot + Gradle + JPA + MySQL + Docker + REST + GitHub" (Clase 3), traducidos a nuestro stack .NET.

**Datos generales**
- Entrega: el link al repositorio de GitHub, con la documentación de decisiones y evidencias.
- Fecha: el enunciado no la define; el equipo fijó **2 de noviembre de 2026** (D-03).
- No hay rúbrica con puntajes; la evaluación es el "Criterio fundamental" (al final).
- IA permitida, pero hay que poder explicar y modificar todo el código entregado.
- El enunciado está pensado para **una entidad con una API REST**. Lo que pide es el piso: CenaAPP implementa la documentación funcional completa (D-03), pero todo lo de este checklist tiene que estar cumplido.

## Parte 1 — Preparación del proyecto
- [x] Proyecto ASP.NET Core MVC (en lugar de Spring Boot) con EF Core, Npgsql y validación (Data Annotations, incluidas en el framework).
- [x] Nombre relacionado con el dominio: `CenaApp` (`CenaApp.sln`, `src/CenaApp.Web`).

## Parte 2 — Elegir un dominio
- [x] Dominio: **Recetas** (está en la lista de la cátedra).
- [x] Permite guardar información en una base de datos, con atributos decididos por el grupo.

## Parte 3 — Diseñar antes de programar (→ Evidencia 1)
- [x] Nombre de la entidad.
- [x] Atributos.
- [x] Tabla Atributo | Tipo (en C#: `int`, `string`, `int?`, `decimal`…).
- [x] Justificación: por qué esos atributos y si alguno sobra.
- [x] Hacerlo **antes** de programar.

## Parte 4 — Base de datos con Docker
- [x] `compose.yaml` en la raíz con imagen oficial de PostgreSQL (en lugar de MySQL).
- [x] Define nombre de base, usuario y contraseña (solo de desarrollo).
- [x] Expone el puerto (`5432:5432`).
- [x] Usa un volumen para no perder los datos.
- [x] Funciona con `docker compose up -d` y se detiene con `docker compose down`.
- [ ] **Evidencia:** captura del contenedor corriendo.

## Parte 5 — Configuración
- [x] Conexión a la base de Docker en `appsettings.Development.json` (equivale a `application.yaml`).
- [x] EF Core configurado en `Program.cs`.
- [x] **Sin contraseñas reales en GitHub:** variables de entorno o User Secrets.

## Parte 6 — Crear la entidad
- [x] Clase C# registrada como `DbSet<T>` en el `DbContext`, con `Id` autogenerado.
- [x] Atributos del diseño de la Parte 3. No copiar una entidad de ejemplo.
- [x] Poder explicar: por qué es una entidad, cuál es el identificador, qué relación hay entre la clase y la tabla.
- [x] Migración creada (`dotnet ef migrations add ...`).

## Parte 7 — Repository
- [x] Obtener todos, obtener por ID, guardar, eliminar (con `DbContext` o una capa Repository, según la decisión de arquitectura).
- [ ] Poder responder: "¿Qué problema nos evita resolver JpaRepository?" (en .NET: qué nos resuelve EF Core / `DbSet`).

## Parte 8 — API REST
- [x] `GET /api/recetas`
- [x] `GET /api/recetas/{id}`
- [x] `POST /api/recetas`
- [x] `DELETE /api/recetas/{id}`

> Aunque las pantallas sean vistas Razor, el enunciado pide estos endpoints JSON. Hay que tener ambas cosas.

## Parte 9 — Crear registros
- [x] El `POST` recibe JSON y el registro queda guardado en PostgreSQL.

## Parte 10 — Consultar registros
- [x] `GET /api/recetas` devuelve todos; `GET /api/recetas/{id}` devuelve uno.
- [x] **ID inexistente:** no puede romper la app. Decidir cómo se maneja (404 con mensaje claro / vista "no encontrado") y **explicar la decisión**.

## Parte 11 — Eliminar
- [x] `DELETE /api/recetas/{id}`.
- [x] Verificar que el registro existe, que se puede eliminar y que después no aparece en el `GET`.

## Parte 12 — Validación
- [x] Validar los datos que llegan (nombre no vacío, email válido si hay, números válidos) con Data Annotations.
- [x] **Al menos dos reglas propias del dominio**, con su justificación. Candidatas de la doc funcional:
  - VAL-15: nombre de receta obligatorio, máx. 80 caracteres, único.
  - VAL-16: tiempo y porciones enteros > 0; calorías enteras ≥ 0.

## Parte 13 — Detectives de errores 🕵️
- [ ] El docente entrega una versión con errores intencionales (en el lenguaje, el framework, controller, repository, configuración de base, Docker, build, endpoints).
- [ ] Ejecutar, identificar, analizar el mensaje, encontrar la causa, corregir y comprobar.
- [ ] **Registro obligatorio** en `docs/ERRORES.md`, por cada error:
  - Error encontrado
  - Mensaje obtenido
  - ¿Qué estaba provocando el error?
  - ¿Cómo lo solucioné?
  - ¿Cómo comprobé que quedó solucionado?
- [x] Usar el mismo formato para los errores reales que aparezcan durante el desarrollo (→ Evidencia 3).

## Parte 14 — Cambio de requisitos (→ Evidencia 4)
El docente asigna uno, sin dar la solución:
- [ ] A: nuevo atributo · B: nuevo endpoint · C: búsqueda (`GET /api/recetas?nombre=...`) · D: segunda entidad · E: nueva validación · F: modificar un endpoint existente.

## Parte 15 — Frontend / AJAX
- [x] Página con formulario, inputs, botón, JavaScript y `fetch()`.
- [x] Recorrido: HTML → JavaScript → `fetch()` → API REST → Controller → EF Core → PostgreSQL.
- [x] La respuesta del backend se muestra en la página.
  - Hecho en **Mi Despensa** (`Views/Despensa/Index.cshtml` + `wwwroot/js/despensa.js`): el formulario "Agregar ingrediente" hace `fetch` y la respuesta (HTML de `_Contenido`) reemplaza las recetas sugeridas. También los favoritos (`js/favoritos.js`, respuesta JSON).

## Parte 16 — Git y GitHub (→ Evidencia 5)
- [x] Repo con `README.md`, archivo de build (`CenaApp.sln` / `.csproj` en lugar de `build.gradle`), `compose.yaml` y `src/`.
- [x] **Al menos 4 commits significativos** durante el desarrollo, no uno solo al final. Ejemplos:
  - Inicialización del proyecto ASP.NET Core
  - Configuración de PostgreSQL con Docker
  - Entidad, DbContext y migración
  - API REST
- [x] Mensajes que describan lo que realmente se hizo.

## Parte 17 — README
- [x] Nombre del proyecto.
- [x] Descripción: qué problema resuelve.
- [x] Tecnologías.
- [x] Cómo ejecutar: `docker compose up -d` → `dotnet ef database update` → `dotnet run --project src/CenaApp.Web`.
- [x] Endpoints: `GET`, `GET /{id}`, `POST`, `DELETE`.
- [x] Al menos un ejemplo de JSON.

## Parte 18 — Evidencia de intervención humana (obligatoria)
| Evidencia | Qué pide | Dónde va |
|---|---|---|
| 1 — Diseño | Diseño inicial de la entidad | `docs/DISENO_ENTIDADES.md` (o sección en `DECISIONES.md`) — a definir |
| 2 — Decisiones | Tres decisiones técnicas con su motivo | `docs/DECISIONES.md` |
| 3 — Error | Un error real del desarrollo y cómo se solucionó | `docs/ERRORES.md` |
| 4 — Cambio de requisito | Qué se cambió al recibir el nuevo requisito | `docs/DECISIONES.md` o sección propia — a definir |
| 5 — Git | Los commits del desarrollo | Historial de GitHub |
| 6 — Explicación oral | Qué hace un archivo, por qué existe, qué pasa si cambia, cómo se relaciona con las otras capas | Defensa oral |

## Parte 19 — Prueba sorpresa
- [ ] Modificación chica en vivo, sin aviso. Ejemplos del enunciado:
  - Endpoint que filtre por nombre que contenga una palabra.
  - Nuevo atributo que aparezca en el `POST` y en el `GET` (→ entidad + migración + DTO).
  - Cambiar la respuesta cuando no existe el ID.

## Parte 20 — Reflexión final
- [ ] ¿Qué parte te resultó más sencilla?
- [ ] ¿Cuál más difícil?
- [ ] ¿Qué error te permitió comprender mejor el framework?
- [ ] ¿Diferencia entre instalar la base directamente y usar Docker?
- [ ] ¿Qué función cumple la herramienta de build (en .NET: `dotnet` CLI y `.csproj`, en lugar de Gradle)?
- [ ] ¿Cuál es el recorrido de una petición desde el Controller hasta la base?
- [ ] ¿Qué harías diferente?

## Entrega final — estructura
El enunciado pide (versión Spring): `README.md`, `build.gradle`, `settings.gradle`, `compose.yaml`, `.gitignore`, `src/main/...`, `src/test/`.

Versión .NET:
- [x] `README.md`
- [x] `CenaApp.sln` (en lugar de `build.gradle` / `settings.gradle`)
- [x] `compose.yaml`
- [x] `Dockerfile`
- [x] `.gitignore`
- [x] `src/CenaApp.Web/`
- [x] `tests/CenaApp.Tests/` (en lugar de `src/test/`)
- [x] Documentación de decisiones y evidencias (`docs/`)

> **Confirmar con el docente** que acepta esta estructura .NET y qué espera en la Parte 13 y la Parte 19.

## Criterio fundamental
"El proyecto debe funcionar, pero que funcione no es suficiente." Cada integrante tiene que poder:
- [ ] explicar su código
- [ ] detectar errores
- [ ] justificar decisiones
- [ ] modificar el proyecto
- [ ] interpretar mensajes de error
- [ ] hacer cambios sin recibir el código de la solución
- [ ] demostrar el proceso con Git
- [ ] responder preguntas sobre la arquitectura

## Desafío extra (opcional)
- [x] Segunda entidad relacionada con la primera (ejemplo: Receta–Categoría).
- [x] Incluye: entidades nuevas, relación en EF Core, cambios en controller/servicio, endpoints nuevos, datos de prueba y README actualizado.
- [ ] Pregunta oral: "¿Por qué elegiste ese tipo de relación y cómo se representa en la base de datos?"
