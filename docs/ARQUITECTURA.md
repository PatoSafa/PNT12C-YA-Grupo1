# Arquitectura

Resumen de cómo está armada la aplicación. Las decisiones y sus motivos están en `docs/DECISIONES.md`.

## Capas

```
Navegador (móvil)
  │  HTML generado por Razor + Bootstrap + wwwroot/css/estilos.css
  │  fetch() solo para despensa y favoritos (D-06)
  ▼
Controllers   (Controllers/)   reciben el pedido, llaman al Service y devuelven View(viewModel) o JSON
  ▼
Services      (Services/)      reglas del negocio: aptitud de recetas, alérgenos calculados, despensa
  ▼
AppDbContext  (Data/)          EF Core: DbSet por entidad + migraciones (D-04)
  ▼
PostgreSQL    (Docker en local, Render en producción)
```

Equivalencia con Spring (clases): Controller = `@Controller`, Service = `@Service`, `DbSet<T>` = `JpaRepository<T>`, Data Annotations = Bean Validation, migraciones = cambios versionados del esquema.

## Carpetas

```
PNT12C-YA-Grupo1/
├── CocinaFacil.sln              solución que agrupa los dos proyectos
├── compose.yaml                 PostgreSQL para desarrollo local
├── .config/dotnet-tools.json    herramienta dotnet-ef (migraciones) en la versión del proyecto
├── docs/                        documentación (funcional, decisiones, arquitectura)
├── src/CocinaFacil.Web/
│   ├── Controllers/             un Controller por área: Home, Cuenta, Recetas, Despensa, Admin...
│   ├── Models/                  entidades del dominio (Receta, Ingrediente, Usuario...)
│   ├── ViewModels/              lo que recibe cada View (nunca entidades con PasswordHash)
│   ├── Services/                reglas del negocio, con interfaz, registradas en Program.cs
│   ├── Data/                    AppDbContext y Migrations/
│   ├── Views/                   vistas Razor por Controller + Shared/_Layout.cshtml
│   ├── wwwroot/                 CSS, JS y librerías estáticas (Bootstrap)
│   ├── appsettings*.json        configuración; la cadena de conexión local va en Development
│   └── Program.cs               registro de servicios, base de datos y rutas
└── tests/CocinaFacil.Tests/     tests con xUnit
```

`Models/`, `Services/` y `Migrations/` se completan a medida que se implementan las tareas.

## Configuración

| Entorno | Cadena de conexión |
|---|---|
| Desarrollo | `appsettings.Development.json` (credenciales solo de desarrollo, iguales a `compose.yaml`) |
| Producción | Variable de entorno `ConnectionStrings__CocinaFacil` en Render (nunca en el repo) |
