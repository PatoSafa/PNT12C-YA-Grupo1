# Evidencias de intervención humana (Parte 18)

Índice de las seis evidencias obligatorias del enunciado: dónde está cada una y qué falta.

| # | Evidencia | Dónde está | Estado |
|---|---|---|---|
| 1 | **Diseño:** diseño inicial de la entidad, antes de programar | [`DISENO_ENTIDADES.md`](DISENO_ENTIDADES.md) (commit `42629e2`, anterior al de las entidades `1c2581c`) | ✅ |
| 2 | **Decisiones:** tres decisiones técnicas con su motivo | [`DECISIONES.md`](DECISIONES.md), de D-01 a D-18. Las tres para presentar están abajo | ✅ |
| 3 | **Error:** un error real y cómo se solucionó | [`ERRORES.md`](ERRORES.md): E-01 a E-04, todos reales y con el formato de la Parte 13 | ✅ |
| 4 | **Cambio de requisito:** qué se cambió al recibir el nuevo requisito | Sección "Cambio de requisito" de este archivo | ⏳ Lo asigna el docente (Parte 14) |
| 5 | **Git:** los commits del desarrollo | Historial de GitHub (`git log --oneline`): 28 commits, uno por tarea, con mensajes que describen lo hecho | ✅ |
| 6 | **Explicación oral:** qué hace un archivo, por qué existe, qué pasa si cambia | Defensa en vivo. Guía para prepararla en [`DEFENSA.md`](DEFENSA.md) | ⏳ Ensayo del equipo (T23) |

Además:
- **Captura de la Parte 4** (contenedor corriendo): ⏳ falta. Sacarla de Docker Desktop (pestaña *Containers*, `cenaapp-postgres` en verde) o de `docker compose ps`, y guardarla en `docs/evidencias/`.
- **Reflexión final (Parte 20):** ⏳ [`REFLEXION.md`](REFLEXION.md), la escribe el equipo.

---

## Evidencia 2 · Las tres decisiones para presentar

1. **Los alérgenos de cada receta se calculan desde sus ingredientes, no se cargan a mano** (D-08). En el prototipo de Figma 3 de 6 recetas tenían los alérgenos mal cargados. Calculándolos, si se corrige un ingrediente se corrigen solas todas sus recetas. Código: `RecetaService.CalcularAlergenos`, con tests en `AptitudRecetasTests`.
2. **Los Services usan el `DbContext` directamente, sin una capa Repository** (D-05). `DbSet<Receta>` ya cumple el papel del `JpaRepository<Receta>` de Spring: otra capa repetiría lo mismo.
3. **Un ID inexistente responde 404 con un mensaje** (D-17). Es el código HTTP que significa "ese recurso no existe". La API devuelve `{ "mensaje": ... }` y las pantallas muestran la vista "Receta no encontrada".

Otras que también se pueden defender: `int` para tiempo y porciones, y `enum` para la dificultad (`DISENO_ENTIDADES.md`); PostgreSQL en vez de MySQL (D-01); la despensa con `fetch` y vista parcial (D-07); la API sin login (D-18).

---

## Evidencia 4 · Cambio de requisito

> Completar cuando el docente asigne el requisito (Parte 14: A nuevo atributo · B nuevo endpoint · C búsqueda · D segunda entidad · E nueva validación · F modificar un endpoint).

- **Requisito recibido:**
- **Qué archivos se cambiaron y por qué:**
- **Cambios en la base (migración):**
- **Cómo se probó:**
- **Commit:**

Ya hay piezas listas para algunos casos. La búsqueda `GET /api/recetas?nombre=...` (C) existe. Para un atributo nuevo (A), la receta de pasos está en `DEFENSA.md`.
