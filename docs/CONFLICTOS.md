# Conflictos entre referencias

Diferencias entre el enunciado, las clases, la documentación funcional (`docs/funcional/`), el prototipo de Figma y el stack elegido. Cada uno necesita una decisión del equipo o una consulta al docente.

## Bloqueantes: enunciado vs. arquitectura

| ID | Conflicto | Fuentes | Impacto | Propuesta |
|---|---|---|---|---|
| C-01 | **El enunciado exige una API REST** (`GET /api/recurso`, `GET /api/recurso/{id}`, `POST`, `DELETE`, con JSON) y la "Prueba sorpresa" pide endpoints en vivo. La decisión D-06 resuelve casi todo con vistas Razor | Enunciado, Partes 8–11, 14 y 19; `docs/DECISIONES.md` D-06 | Sin API REST no se cumplen 4 partes obligatorias | **Nueva decisión D-11:** sumar un controller de API (`/api/recetas`) con `GET`, `GET/{id}`, `POST` y `DELETE` sobre los mismos Services que usan las vistas. Las pantallas siguen en Razor |
| C-02 | El enunciado está escrito para **Java + Spring Boot + Gradle + MySQL**; el proyecto usa .NET + PostgreSQL | Enunciado, Partes 1, 4, 16, 17 y "Entrega final"; `CLAUDE.md` sección 3 | Riesgo de que no se acepte la entrega | Confirmar con el docente (ya marcado en `CLAUDE.md`, sección 8). La Clase 6 usa PostgreSQL en producción, lo que ayuda a justificar el motor |
| C-03 | La Parte 3 pide **diseñar la entidad antes de programar** (Evidencia 1) | Enunciado, Partes 3 y 18 | La evidencia tiene que ser anterior al código de las entidades | Escribir `docs/DISENO_DATOS.md` (entidades, atributos, tipos C# y justificación) antes de crear `Models/` |

## Clases vs. stack

| ID | Conflicto | Fuentes | Propuesta |
|---|---|---|---|
| C-04 | La **capa de Services** no aparece en las clases: el Controller llama directo al Repository. El primer `CLAUDE.md` decía que se había visto en las Clases 3 y 5 | `_referencias/clases/Clase 3`, `Clase 5`; D-02 | Mantener D-02, pero prepararse para explicarla en la defensa (`docs/TECNOLOGIAS.md`, "Lo que usamos y no se vio en clase") |
| C-05 | Las **anotaciones de validación** no tienen guía de clase: solo se agrega la dependencia Validation. El enunciado dice "las anotaciones trabajadas en clase" | Clase 3 (`guia_springboot_gradle_docker_github.html`); Enunciado, Parte 12 | Usar Data Annotations y documentar el equivalente (`@NotBlank` → `[Required]`) |
| C-06 | El **login de la Clase 5** guarda el usuario en `sessionStorage` con jQuery; el proyecto usa autenticación por cookie | Clase 5 (`paso_a_paso_home_ajax.html`); `CLAUDE.md` sección 3 | Mantener la cookie (con `sessionStorage` cualquiera podría hacerse pasar por admin) y explicarlo en la defensa |

## Documentación funcional vs. prototipo

**Manda la documentación funcional.** El prototipo define el aspecto visual.

| ID | Diferencia | Documentación funcional | Prototipo (`_referencias/figma-export/`) | Acción |
|---|---|---|---|---|
| C-07 | Nombre de la app | CocinaFácil | CenAPP | Definir el nombre final |
| C-08 | Umbral de "Casi listas" | 50% o más (VN-12) | 25% o más (`Despensa.tsx`, línea 126) | Implementar 50% |
| C-09 | Restricciones | Filtro estricto (VN-06) | Deja pasar toda receta que no tenga restricciones cargadas (`Home.tsx`, línea 33); por eso un perfil vegetariano ve "Tacos de Pescado" | Implementar filtro estricto |
| C-10 | Alérgenos | Se calculan desde los ingredientes (VN-04) | Cargados a mano en `data/recipes.ts`, con errores en 3 de 6 recetas | Calcular en `RecetaService` |
| C-11 | Ingredientes de la despensa | Solo del catálogo, con sinónimos (VAL-13) | Permite texto libre y guarda en `localStorage` | Solo catálogo; guardar en la base (D-07) |
| C-12 | Catálogos | 16 restricciones, 13 alérgenos, 12 gustos | 7 restricciones, 8 alérgenos, 8 gustos (`Onboarding.tsx`) | Usar los catálogos de la documentación |
| C-13 | Tiendas | Sin botón "Llamar"; ruta a pie; comercios reales | Botón "Llamar", 5 comercios fijos y mapa Leaflet | 1ª entrega: links a Google Maps (D-09) |
| C-14 | Registro, login, administración | Incluidos | No existen | Diseñarlas con los componentes de `docs/DISENO.md` |
| C-15 | Persistencia | Base de datos por usuario | Estado en memoria (`UserContext.tsx`) y `localStorage` | Base de datos (D-07) |
| C-16 | Pasos de la receta | Casilla por paso y celebración | Lista sin casillas | Implementar casillas en JS local (D-06) |

## Documentación funcional vs. stack

| ID | Conflicto | Acción |
|---|---|---|
| C-17 | La documentación funcional dice "frontend en React + TypeScript + Tailwind CSS" (sección 2.1 y RNF21) | Actualizar la documentación funcional al stack .NET (Razor + Bootstrap) |
| C-18 | El alcance de la documentación funcional (14 casos de uso, administración, mapas, 118 recetas) supera el mínimo del enunciado (una entidad con CRUD) | Resuelto con D-03 (alcance por entregas) |
| C-19 | La sección 2 de `CLAUDE.md` dice del export de Figma "usalo solo como referencia" y, en la misma línea, "**Usa de base ese código**" | Corregir la frase: el código del prototipo no se usa (la sección 3 descarta React y Tailwind) |
