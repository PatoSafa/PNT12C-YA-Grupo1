# PNT12C-YA-Grupo1 · CenaAPP

Repositorio que se utilizará para la materia PNT1 - 2C - YA - ORT.

CenaAPP es una app web móvil de recetas personalizadas según el perfil del usuario (restricciones, alergias y gustos), con despensa virtual y búsqueda de comercios cercanos.

## Tecnologías

C# / .NET 10, ASP.NET Core MVC con vistas Razor, Bootstrap, Entity Framework Core y PostgreSQL (Docker). Detalle en `docs/ARQUITECTURA.md` y `docs/DECISIONES.md`.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Visual Studio Code con la extensión C# Dev Kit

## Cómo correr el proyecto en local

```bash
# 1. Levantar PostgreSQL
docker compose up -d

# 2. Restaurar la herramienta de migraciones (solo la primera vez)
dotnet tool restore

# 3. Aplicar las migraciones (cuando existan)
dotnet ef database update --project src/CenaApp.Web

# 4. Correr la app
dotnet run --project src/CenaApp.Web
```

Abrir http://localhost:5224. La página de inicio indica si la app se conectó a la base de datos.

Para correr los tests: `dotnet test`.

Para apagar la base: `docker compose down` (los datos se conservan en el volumen `cenaapp-datos`).

## Documentación

- `docs/funcional/` — documentación funcional del sistema.
- `docs/DECISIONES.md` — decisiones de arquitectura.
- `docs/ARQUITECTURA.md` — capas y carpetas.
- `docs/CONSIGNA.md` — requisitos del enunciado como checklist.
- `docs/TECNOLOGIAS.md` — stack y equivalencias con lo visto en clase.
- `docs/DISENO.md` — pantallas, rutas, paleta y componentes del prototipo.
- `docs/CONFLICTOS.md` — diferencias entre enunciado, documentación funcional, prototipo y stack.
