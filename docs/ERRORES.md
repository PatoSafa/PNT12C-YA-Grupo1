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
