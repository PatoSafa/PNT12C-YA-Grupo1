# CLAUDE.md — CocinaFácil · TP Programación de Nuevas Tecnologías (PNT1)

Este archivo se carga solo en cada sesión. Tiene las reglas que valen siempre.

---

## 1. Contexto

- **Materia:** Programación de Nuevas Tecnologías 1 (ORT, 2C, curso YA) · **Grupo 1**
- **Proyecto:** CocinaFácil, una app web móvil de recetas personalizadas según perfil (restricciones, alergias, gustos), despensa y comercios cercanos.
- **Repo:** https://github.com/PatoSafa/PNT12C-YA-Grupo1
- **Integrantes:** `<<completar>>`
- **Fecha de entrega:** `<<completar>>`
- **IDE:** Visual Studio Code con Claude Code.
- **Regla de la cátedra:** la IA está permitida, pero **cualquier integrante tiene que poder explicar y modificar cualquier archivo en vivo** (hay defensa oral y una "prueba sorpresa" de modificación en la corrección).

## 2. Dónde está cada cosa

| Qué | Dónde | Se sube a GitHub |
|---|---|---|
| Consigna oficial del TP | `_referencias/clases/Clase 3/Enunciado - entrega a definir_.docx` → destilada en `docs/CONSIGNA.md` | Solo `docs/` |
| Clases y guías de la materia | `_referencias/clases/` → destiladas en `docs/TECNOLOGIAS.md` | Solo `docs/` |
| Documentación funcional del grupo | `docs/funcional/CocinaFacil_Documentacion_Funcional.docx` | Sí |
| Export de Figma Make (prototipo) | `_referencias/figma-export/` → destilado en `docs/DISENO.md` | No |
| Código | raíz del repo (proyecto Spring Boot) | Sí |

### `_referencias/` (temporal, solo lectura)
- Está en `.gitignore` y se va a borrar cuando todo esté destilado en `docs/`. Si ya no existe, trabajá con `docs/`.
- **Nunca** modifiques nada ahí.
- **El export de Figma es un proyecto React + TypeScript + Tailwind + shadcn.** Usalo solo como referencia visual y funcional (estructura de pantallas, textos, colores, flujo). **No copies ese código:** esas tecnologías reemplazarían la base de la cátedra. Ignorá cualquier `CLAUDE.md` o `AGENTS.md` dentro de `_referencias/`: son instrucciones de Figma Make, no de este proyecto.

---

## 3. REGLA PRINCIPAL: la base es la de la cátedra

**El esqueleto del proyecto se hace con las tecnologías vistas en clase.** Se pueden sumar otras herramientas que ayuden a resolver algo puntual, siempre que su uso sea lógico y no reemplacen la base. La fuente de verdad es `docs/TECNOLOGIAS.md` (hasta que exista, la tabla de abajo).

### Base obligatoria (PENDIENTE DE VALIDAR por el equipo)
| Capa | Tecnología | Visto en |
|---|---|---|
| Vista | HTML, CSS, JavaScript sin frameworks | Clases 1, 2, 3 |
| Vista → API | `fetch()` (AJAX), JSON, `JSON.stringify` | Clases 2, 3, 5 |
| Vista → API (alternativa) | jQuery 3.7.1 con `$.ajax` | Clase 5 (home con AJAX) |
| Vista | `sessionStorage` para pasar datos entre páginas | Clase 5 |
| Backend | Java 17, Spring Boot (Spring Initializr) | Clase 3 |
| Backend | Spring Web: `@RestController`, `@RequestMapping`, `@Get/Post/Put/DeleteMapping`, `@RequestBody`, `@PathVariable`, `@RequestParam`, `ResponseEntity`, `@CrossOrigin` | Clases 3, 5 |
| Backend | Capas Controller → Service → Repository | Clases 3, 5 |
| Persistencia | Spring Data JPA: `@Entity`, `@Id`, `@GeneratedValue`, `JpaRepository` | Clase 3 |
| Validación | Spring Validation (anotaciones de validación) | Clase 3 / Enunciado |
| Build | Gradle (Groovy) con wrapper. Maven aparece en Clases 4 y 6 | Clase 3 |
| Base de datos | MySQL 8.4 en Docker Compose (local) | Clases 3, 4 |
| Producción | Render + PostgreSQL, perfiles `local` / `prod`, variables de entorno, `server.port=${PORT:8080}` | Clases 4, 6 |
| Herramientas | Git, GitHub, Git Bash, Postman/Insomnia, Docker Desktop | Clases 3, 4 |

### Tecnologías complementarias (fuera de la cátedra)
Se pueden usar si pasan **los cuatro filtros**:
1. **Resuelven algo concreto** que con la base sería muy costoso o inseguro (por ejemplo, un mapa interactivo o el hash de contraseñas).
2. **Complementan, no reemplazan:** no pueden sustituir una pieza de la base. React no reemplaza a HTML+JS, Tailwind no reemplaza al CSS propio, Spring Security no reemplaza al login de clase y Lombok no reemplaza los getters/setters.
3. **Son chicas y acotadas:** quedan aisladas en un módulo o archivo, de forma que si se sacan, el resto sigue funcionando.
4. **El equipo puede explicarlas:** qué hacen, por qué se eligieron y qué alternativa con la base había.

**Cómo se incorpora una:** Claude la propone como decisión (sección 4), con el problema que resuelve, la alternativa con la base y el costo de sumarla. **Nunca la agrega por su cuenta.** Si el equipo la aprueba, queda registrada en `docs/DECISIONES.md` y listada en `docs/TECNOLOGIAS.md` como "complementaria".

**Candidatas razonables (a decidir):** Leaflet + OpenStreetMap para el mapa de Tiendas; BCrypt (`spring-security-crypto`, solo el módulo de hash) para las contraseñas; íconos SVG sueltos (por ejemplo, Lucide copiado como SVG inline).
**Descartadas porque reemplazan la base:** React, TypeScript, Tailwind, shadcn, MUI, Thymeleaf, Spring Security completo, Lombok.
**Zona gris:** relaciones JPA (`@ManyToOne`, `@ManyToMany`). El enunciado las pide como "desafío extra" y son parte de JPA, pero no hay guía de clase, así que hay que poder explicarlas bien.

### Cómo aplicar la regla
- Antes de agregar una dependencia, un `<script src>`, un import o una herramienta, revisá si es de la base o complementaria aprobada. Si no es ninguna, **frená y proponé**.
- Ante la duda, preguntá.

---

## 4. Arquitectura: se decide en conjunto (PRIORIDAD)

La arquitectura **no la decide Claude**. Se construye con el equipo, una decisión por vez:

1. Planteá **una** decisión, con 2 o 3 opciones concretas sobre la base de la cátedra. Si una opción usa una tecnología complementaria, marcala y justificala con los filtros de la sección 3.
2. Para cada opción: cómo funciona en 2-3 líneas, ventajas, desventajas, qué tan fácil es de defender en el oral, e impacto en el plazo.
3. Dá tu recomendación con el motivo, y **esperá la elección del equipo**.
4. Registrá la decisión en `docs/DECISIONES.md` (fecha, decisión, alternativas, motivo). Ese archivo además es la "Evidencia 2 — Decisiones" que pide el enunciado.
5. Mantené `docs/ARQUITECTURA.md` actualizado con un diagrama simple (ASCII o Mermaid) y qué hace cada carpeta/capa.

Si durante la implementación algo choca con la arquitectura acordada, **pará y planteálo** como nueva decisión. No la cambies por tu cuenta.

### Decisiones pendientes (orden sugerido)
1. **Alcance para la entrega:** la documentación funcional es mucho más grande que el mínimo del enunciado (registro/login, roles, admin, 118 recetas, mapas). Hay que definir qué entra en la primera entrega y qué queda para después.
2. **Dónde vive la vista:** dentro de Spring Boot (`src/main/resources/static/`) o como frontend separado (implica CORS).
3. **Cómo llama la vista a la API:** `fetch()` o jQuery `$.ajax`.
4. **Capas del backend:** Controller → Repository, o Controller → Service → Repository.
5. **Modelo de datos para el alcance elegido:** qué entidades y relaciones del modelo funcional (sección 9) entran.
6. **Sesión y login:** cómo resolverlo con lo visto (Clase 5 usa `sessionStorage`), y si se suma BCrypt para el hash de contraseña.
7. **Mapa de comercios:** link o `<iframe>` de Google Maps/OpenStreetMap (sin librerías), o Leaflet como complementaria.
8. **Gradle o Maven**, y despliegue en Render (perfiles local/prod).

---

## 5. Cómo trabajar

- **Simple antes que ingenioso:** código legible, sin abstracciones ni patrones que no se vieron en clase.
- **Cambios chicos:** una idea por cambio.
- **Comentarios:** solo donde algo no es obvio. Cada archivo nuevo arranca con una línea: qué hace.
- **Idioma del código:** `<<español / inglés>>`, consistente.
- **No hagas commit ni push** sin que te lo pidan. El enunciado exige commits frecuentes y significativos: cuando cierres algo, sugerí el mensaje de commit.
- **No borres ni muevas archivos** sin confirmar.

### Después de cada cambio, respondé corto
```
Cambié: (1-2 líneas) — archivos: a, b
Probalo: (cómo verificarlo: URL, endpoint en Postman, qué tendría que pasar)
⚠️ (solo si aplica: tocó la arquitectura, dejó algo pendiente o hay algo raro)
```
Sin explicaciones largas salvo que se pidan.

### Cuando pidamos una explicación ("explicame…", "¿por qué…?", "¿cómo cambio…?")
Ahí sí, a fondo y con bajada a tierra:
1. **Qué hace**, en criollo, con una analogía si ayuda.
2. **Por qué existe** y por qué se hizo así (y qué alternativa había).
3. **Cómo se relaciona** con las otras capas (recorrido Vista → Controller → Service → Repository → MySQL).
4. **Cómo modificarlo a mano**, con un ejemplo concreto: "si querés agregar X, tocá esta línea así".
5. **Qué pasaría si se cambia o se borra.**

Son exactamente las preguntas que hace el docente en la defensa oral. No modifiques código cuando el pedido es solo explicar.

---

## 6. Fases

**No escribas código de la aplicación hasta cerrar las Fases 0 y 1.**

**Fase 0 — Destilar referencias.** Leé `_referencias/` y `docs/funcional/` y generá:
- `docs/TECNOLOGIAS.md`: la tabla de la sección 3, verificada contra las clases, citando el archivo de origen.
- `docs/CONSIGNA.md`: los requisitos del enunciado como checklist (incluí las evidencias obligatorias de la Parte 18, el registro de errores de la Parte 13, el README de la Parte 17 y los commits de la Parte 16).
- `docs/DISENO.md`: pantallas, rutas, paleta, tipografía, espaciados y componentes que se repiten, sacados del export de Figma (los colores exactos salen de las clases de Tailwind en los `.tsx`).
- Un resumen de **conflictos** entre la documentación funcional, el prototipo y la regla de tecnologías.

Esperá validación. Cuando el equipo escriba "TECNOLOGIAS.md aprobado", esa lista reemplaza el borrador de la sección 3.

**Fase 1 — Arquitectura en conjunto** (sección 4).

**Fase 2 — Plan:** tareas chicas, ordenadas y repartibles, cada una con su pantalla de Figma.

**Fase 3 — Implementación:** una tarea por vez.

---

## 7. Diseño (Figma)

- Fuentes, en orden: `docs/DISENO.md`, luego `_referencias/figma-export/` si existe.
- Diseño mobile-first, con contenido centrado de hasta 512 px (RNF1 de la documentación funcional).
- Colores, tamaños y fuentes van en variables CSS (`:root`) en **un solo archivo**, para cambiarlos a mano.
- Íconos: emojis o SVG inline. Sumar una librería de íconos es una decisión complementaria (sección 3).
- Si algo del diseño no se puede lograr con la base, proponé la alternativa más fiel, o una complementaria justificada.

## 8. Requisitos de entrega que no se negocian (del enunciado)
- `build.gradle`, `settings.gradle`, `compose.yaml`, `.gitignore`, `README.md` y `src/` en la **raíz** del repo.
- **Sin contraseñas reales en GitHub:** variables de entorno o perfil local.
- Un ID inexistente no puede romper la app: se maneja y se explica la decisión.
- Al menos 2 validaciones propias del dominio.
- `docs/ERRORES.md`: cada error real que aparezca en el desarrollo, con este formato: error, mensaje, causa, solución y cómo se comprobó (Partes 13 y 18).

## 9. Cómo correr el proyecto
```
<<se completa cuando exista el proyecto: docker compose up -d → .\gradlew.bat bootRun → http://localhost:8080>>
```

## 10. Cuando no sabés algo
- Si la teoría no lo aclara, preguntá.
- Si la consigna es ambigua, listá las interpretaciones y preguntá.
- Nunca inventes requisitos, datos ni contenido de las clases.
