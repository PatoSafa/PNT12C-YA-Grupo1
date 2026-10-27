# Imagen de CenaAPP para Render (Render no tiene runtime nativo de .NET, por eso se despliega como contenedor).
# Dos etapas: la primera compila con el SDK, la segunda solo lleva lo necesario para correr (más chica).

# ---------- 1. Compilar ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacion
WORKDIR /src

# Primero solo el .csproj y restore: si no cambian los paquetes, Docker reusa esta capa y el build es más rápido.
COPY src/CenaApp.Web/CenaApp.Web.csproj src/CenaApp.Web/
RUN dotnet restore src/CenaApp.Web/CenaApp.Web.csproj

COPY src/CenaApp.Web/ src/CenaApp.Web/
RUN dotnet publish src/CenaApp.Web/CenaApp.Web.csproj -c Release -o /app --no-restore

# ---------- 2. Ejecutar ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=compilacion /app .

# En producción la cadena de conexión y las claves de los usuarios de prueba llegan por variables de entorno
# (ConnectionStrings__CenaApp, DatosIniciales__ClaveAdmin...), nunca desde el repositorio.
ENV ASPNETCORE_ENVIRONMENT=Production
# Puerto por defecto; Render lo cambia con la variable PORT (ver Program.cs).
EXPOSE 8080

ENTRYPOINT ["dotnet", "CenaApp.Web.dll"]
