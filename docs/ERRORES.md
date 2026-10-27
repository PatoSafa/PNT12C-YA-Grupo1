# Registro de errores

Errores reales que aparecieron durante el desarrollo, con el formato que pide el enunciado (Partes 13 y 18, Evidencia 3). Se agrega uno nuevo cada vez que algo falla y se resuelve.

---

## E-01 · Docker Desktop no arranca después de instalarlo
- **Fecha:** 2026-10-26
- **Error encontrado:** al correr cualquier comando de Docker (`docker info`, `docker compose up -d`), el motor no respondía. Docker Desktop mostraba una ventana de error con los botones "Restart" y "Quit".
- **Mensaje obtenido:**
  ```
  Error response from daemon: Docker Desktop is unable to start
  ```
  y en el log de Docker (`%LOCALAPPDATA%\Docker\log\host\com.docker.backend.exe.log`):
  ```
  engine linux/wsl failed to start: checking preconditions: checking WSL version:
  executing wsl --version ... stderr: "WSL está finalizando una actualización..."
  ```
- **¿Qué estaba provocando el error?** Docker Desktop en Windows usa WSL 2 (el subsistema de Linux) para correr los contenedores. Después de reiniciar, WSL todavía estaba terminando de instalar una actualización. Docker arrancó antes de que terminara, el comando `wsl --version` no respondió a tiempo, y Docker se dio por fallado.
- **¿Cómo lo solucioné?** Verifiqué que WSL ya respondía (`wsl --version` mostraba la versión 3.0.1.0), cerré Docker Desktop y lo volví a abrir. Esta vez encontró WSL listo y el motor arrancó.
- **¿Cómo comprobé que quedó solucionado?**
  1. `docker info` devolvió la versión del servidor (29.9.2).
  2. `docker compose up -d` descargó `postgres:17` y levantó el contenedor `cenaapp-postgres`, que quedó en estado `healthy`.
  3. Con `dotnet run --project src/CenaApp.Web`, la página de inicio mostró "Base de datos PostgreSQL: conectada."
- **Qué aprendí:** el mensaje que se ve en pantalla ("unable to start") no dice la causa. La causa real estaba en el log de Docker. Leer los logs antes de reinstalar ahorra tiempo.

## E-02 · La migración no se crea: "skip navigation doesn't have a foreign key"
- **Fecha:** 2026-10-26
- **Error encontrado:** al cambiar los nombres de las columnas de las tablas intermedias N:M (para que `Favoritos` tenga `RecetaId` en vez de `FavoritosId`), `dotnet ef migrations add` y `dotnet ef database update` dejaron de funcionar.
- **Mensaje obtenido:**
  ```
  Unable to create a 'DbContext' of type 'AppDbContext'. The exception 'The skip navigation
  'Alergeno.Ingrediente' doesn't have a foreign key associated with it. Every skip navigation
  must have a configured foreign key.' was thrown while attempting to create an instance.
  ```
- **¿Qué estaba provocando el error?** En `UsingEntity(tabla, ladoA, ladoB)` de EF Core, el **primer** lambda configura la clave foránea hacia la entidad **relacionada** (la de la lista, ej. `Receta`) y el **segundo** hacia la entidad **dueña** (ej. `Usuario`). Los había puesto al revés, así que EF Core quedaba con una relación sin clave foránea.
- **¿Cómo lo solucioné?** Invertí el orden en el método `TablaIntermedia` de `AppDbContext` y le puse nombres claros a los parámetros (`ladoRelacionado`, `ladoDuenio`) para que no vuelva a pasar. Después rehice la migración `Inicial` (`database update 0` → `migrations remove` → `migrations add Inicial` → `database update`).
- **¿Cómo comprobé que quedó solucionado?** La migración se aplicó, y en PostgreSQL las tablas intermedias quedaron con columnas `UsuarioId` / `RecetaId`, `UsuarioId` / `IngredienteId`, etc. (consulta a `information_schema.columns`).
- **Qué aprendí:** el mensaje nombra la relación que falla (`Alergeno.Ingrediente`), y eso indica qué lado quedó mal configurado.

## E-03 · Warning MSB3277: conflicto de versiones de EF Core en el proyecto de tests
- **Fecha:** 2026-10-26
- **Error encontrado:** el build terminaba bien pero con una advertencia en `CenaApp.Tests`.
- **Mensaje obtenido:**
  ```
  warning MSB3277: Existía un conflicto entre "Microsoft.EntityFrameworkCore, Version=10.0.4.0"
  y "Microsoft.EntityFrameworkCore, Version=10.0.13.0".
  ```
- **¿Qué estaba provocando el error?** La app usa EF Core 10.0.13, que llega por el paquete `Design`. Pero `Design` está marcado como privado (solo para la herramienta de migraciones), así que esa versión no pasa al proyecto de tests. Los tests recibían la versión que pide Npgsql (10.0.4), y quedaban dos versiones distintas.
- **¿Cómo lo solucioné?** En `CenaApp.Web.csproj` agregué una referencia explícita a `Microsoft.EntityFrameworkCore.Relational` 10.0.13, para que la app y los tests usen la misma versión.
- **¿Cómo comprobé que quedó solucionado?** `dotnet build CenaApp.sln` → "0 Advertencia(s), 0 Errores", y `dotnet test` → 1 de 1 correcto.

## E-04 · NullReferenceException al guardar una receta sin pasos (admin)
- **Fecha:** 2026-10-26
- **Error encontrado:** en el formulario de alta de recetas del administrador, al enviar el textarea de pasos vacío, la app mostraba la página de error en vez del mensaje de validación.
- **Mensaje obtenido:**
  ```
  System.NullReferenceException: Object reference not set to an instance of an object.
     at CenaApp.Web.Controllers.AdminRecetasController.PrepararYValidarAsync(RecetaFormularioViewModel formulario)
     in ...\Controllers\AdminRecetasController.cs:line 142
  ```
- **¿Qué estaba provocando el error?** La propiedad `PasosTexto` se inicializa con `""`, pero cuando un campo del formulario llega vacío, el *model binder* de ASP.NET Core lo convierte en `null` (por defecto, `ConvertEmptyStringToNull = true`). Entonces `formulario.PasosTexto.Split(...)` se ejecutaba sobre `null`.
- **¿Cómo lo solucioné?** Cambié la línea por `(formulario.PasosTexto ?? "").Split(...)`: si no llegó texto, se toma como "sin pasos" y la validación `[MinLength(1)]` del DTO muestra "La receta necesita al menos un paso."
- **¿Cómo comprobé que quedó solucionado?** Volví a enviar el formulario con el nombre repetido, el tiempo en 0, sin ingredientes y sin pasos (con `curl`). Ahora responde el formulario con los mensajes "El tiempo tiene que estar entre 1 y 600 minutos", "La receta necesita al menos un ingrediente" y "La receta necesita al menos un paso", y en el log no aparece ninguna excepción. El de nombre repetido aparece en cuanto se corrigen esos campos, porque las reglas del servicio se controlan después de las Data Annotations.
- **Qué aprendí:** la línea exacta del error está en el stack trace (`line 142`). Además, un valor por defecto en la clase (`= ""`) no garantiza nada: lo que llega del formulario lo pisa.
