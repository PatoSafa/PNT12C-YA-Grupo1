# Diseño de entidades (antes de programar)

Responde la Parte 3 del enunciado ("Diseñar antes de programar") y es la **Evidencia 1 — Diseño** de la Parte 18. Se escribió el 26/10/2026, antes de crear las clases (T04).

La entidad principal es **Receta**: es el "recurso" de la API que pide el enunciado (`/api/recetas`). El resto de las entidades se resume al final; el diagrama completo está en `ARQUITECTURA.md` §4.

---

## 1. Entidad principal: `Receta`

### Atributos y tipos

| Atributo | Tipo C# | Obligatorio | Regla |
|---|---|---|---|
| `Id` | `int` | sí (lo genera la base) | identificador autoincremental |
| `Nombre` | `string` | sí | máx. 80 caracteres, **único** |
| `Descripcion` | `string?` | no | máx. 300 caracteres |
| `Emoji` | `string` | sí | un emoji (por ejemplo 🥗) |
| `TiempoMin` | `int` | sí | entero **mayor a 0** |
| `Porciones` | `int` | sí | entero **mayor a 0** |
| `Dificultad` | `Dificultad` (enum: `Facil`, `Media`, `Dificil`) | sí | uno de los tres valores |
| `Calorias` | `int` | sí | entero **mayor o igual a 0** (por porción) |
| `CategoriaId` | `int` | sí | clave foránea a `Categoria` |

Relaciones (no son columnas de la tabla `Recetas`, EF Core las arma con otras tablas):

| Propiedad | Tipo C# | Relación |
|---|---|---|
| `Categoria` | `Categoria` | muchas recetas → una categoría (N:1) |
| `Pasos` | `List<Paso>` | una receta → muchos pasos, con orden (1:N) |
| `Ingredientes` | `List<RecetaIngrediente>` | receta ↔ ingrediente, con cantidad y unidad (N:M con datos propios) |
| `Gustos` | `List<Gusto>` | receta ↔ gusto (N:M simple) |

Los favoritos se navegan desde el usuario (`Usuario.Favoritos`); la receta no necesita la lista inversa.

### Por qué estos atributos
- **`Nombre`** es lo que el usuario busca y ve en cada tarjeta. Es único porque dos recetas con el mismo nombre confundirían la búsqueda y el admin (VAL-15).
- **`Descripcion`** es la línea corta que aparece debajo del nombre en el Inicio. Es opcional: una receta se entiende sin ella.
- **`Emoji`** reemplaza la foto (D-04: emojis, no fotos). Se guarda como texto porque un emoji es un carácter más.
- **`TiempoMin`** y **`Porciones`** son `int` porque se cuentan en unidades enteras: nadie cocina 12,5 minutos ni sirve 2,3 porciones. Tienen que ser mayores a 0: una receta de 0 minutos o 0 porciones no tiene sentido (VAL-16).
- **`Dificultad`** es un `enum` y no un `string` libre: solo hay tres valores posibles, y así no pueden aparecer "facil", "Fácil" y "FACIL" como si fueran distintos. En la base se guarda como texto (`"Facil"`) para que se lea sin traducir números.
- **`Calorias`** es `int` porque es una estimación por porción; los decimales no aportan nada. Puede ser 0 (por ejemplo, una infusión), pero no negativa.
- **`CategoriaId`** separa las categorías en su propia tabla en vez de escribirlas como texto en cada receta: si se renombra "Ensalada", se cambia en un solo lugar.
- **`Pasos`** es una tabla aparte (1:N) y no un texto largo, porque cada paso tiene su número de orden y en el Detalle se marcan uno por uno.

### Atributos que se consideraron y se descartaron
| Atributo | Por qué no |
|---|---|
| `Alergenos` (guardados en la receta) | Se **calculan** desde los ingredientes (RF8, VN-04). Guardarlos dos veces permite que no coincidan: es justo el error que tenía el prototipo de Figma (3 de 6 recetas mal cargadas). |
| `Restricciones` aptas (guardadas) | Mismo motivo: se calculan. Si se corrige un ingrediente, todas sus recetas quedan bien solas. |
| `Imagen` / foto | Fuera de alcance (D-04: emojis). |
| `CaloriasEstimadas` (sí/no) | En esta base todas las calorías son estimadas, así que el campo siempre valdría lo mismo. |
| `Fuente` y `Nota` | Sirven para documentar de dónde salió la receta, pero ninguna pantalla los muestra. |
| `Vegana`, `SinGluten`… (un `bool` por restricción) | Habría que agregar una columna por cada restricción nueva. Con la relación Ingrediente ↔ Restriccion alcanza con cargar datos, sin tocar el modelo. |

### Validaciones propias del dominio (Parte 12)
Se eligen estas dos, que vienen de la documentación funcional:
1. **Nombre obligatorio, de hasta 80 caracteres y único** (VAL-15). Con `[Required]` y `[StringLength(80)]`, más un índice único en la base. **Por qué:** el nombre es lo que identifica a la receta para el usuario. Vacío no se puede mostrar ni buscar, y repetido genera dos tarjetas iguales.
2. **Tiempo y porciones mayores a 0, calorías mayores o iguales a 0** (VAL-16). Con `[Range(1, …)]` y `[Range(0, …)]`. **Por qué:** con valores en 0 o negativos, la tarjeta mostraría "0 min" o "-2 porciones". Además, el tiempo sirve para el filtro "Rápido".

---

## 2. Resto de las entidades (resumen)

| Entidad | Atributos principales | Por qué existe |
|---|---|---|
| `Usuario` | `Id`, `Email` (único), `PasswordHash`, `Rol` (enum `Usuario`/`Admin`), `Nombre` (máx. 50), `Nivel` (enum `Principiante`/`Intermedio`/`Avanzado`), `CreadoEn` (`DateTime`) | Login, perfil, favoritos y despensa de cada persona. Se guarda el **hash** de la contraseña, nunca la contraseña (RNF13). |
| `Ingrediente` | `Id`, `Nombre` (único), `Grupo`, `Sinonimos` (texto separado por comas), `CuentaEnDespensa` (`bool`) | Es la base del cálculo de alérgenos y restricciones. `CuentaEnDespensa = false` para sal, agua, etc., que no cuentan en el porcentaje (VN-10). |
| `RecetaIngrediente` | `RecetaId`, `IngredienteId`, `Cantidad` (`decimal?`, vacío = "a gusto"), `Unidad`, `Nota` | Es la tabla intermedia, pero tiene clase propia porque guarda datos de la relación. `decimal?` porque hay cantidades como 0,5 huevo o "a gusto". |
| `Paso` | `Id`, `RecetaId`, `Orden` (`int`), `Texto` (máx. 500) | Pasos numerados de una receta. |
| `Categoria` | `Id`, `Nombre` | 7 categorías (Plato principal, Ensalada…). |
| `Restriccion` | `Id`, `Nombre`, `Descripcion` | 5 restricciones (D-04). |
| `Alergeno` | `Id`, `Nombre` | 5 alérgenos (D-04). |
| `Gusto` | `Id`, `Nombre`, `Tipo` (enum `Cocina`/`Estilo`) | 12 gustos, que ordenan las recetas (VN-08). |

Relaciones N:M sin clase propia (EF Core crea la tabla intermedia): Usuario↔Restriccion, Usuario↔Alergeno, Usuario↔Gusto, Usuario↔Ingrediente (despensa), Usuario↔Receta (favoritos), Ingrediente↔Alergeno, Ingrediente↔Restriccion (incompatibles), Receta↔Gusto.

**Comercio** no es una entidad: los comercios son una lista fija en `ComercioService` (D-08, D-15).

---

## 3. Preguntas que pide el enunciado (Parte 6), respondidas de antemano
- **¿Por qué `Receta` es una entidad?** Porque representa algo del dominio que se guarda en la base y tiene identidad propia: cada receta se distingue de las otras por su `Id`, aunque cambie de nombre.
- **¿Qué atributo es el identificador?** `Id`. EF Core lo reconoce como clave primaria por convención (se llama `Id`) y PostgreSQL lo genera solo, como `@Id @GeneratedValue(IDENTITY)` en Spring.
- **¿Qué relación hay entre la clase y la tabla?** EF Core mapea la clase `Receta` a la tabla `Recetas`: cada propiedad simple es una columna, cada objeto `Receta` es una fila y cada `List<>` es una relación con otra tabla. La tabla se crea con una **migración**, que queda versionada en Git.
