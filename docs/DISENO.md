# Diseño

Pantallas, rutas, paleta, tipografía y componentes del prototipo, para reproducirlos con vistas Razor, Bootstrap y `wwwroot/css/estilos.css` (D-08).

Fuentes: export de Figma Make en `_referencias/figma-export/` (componentes en `src/app/components/*.tsx`, estilos en `src/styles/theme.css`) y prototipo publicado en https://lid-olive-63487655.figma.site. Las capturas de cada pantalla están en la sección 5 de la documentación funcional (`docs/funcional/`).

> El prototipo usa React + Tailwind + Lucide. Acá se toman **solo** las decisiones visuales y de flujo; el código no se copia (`CLAUDE.md`, sección 2).

## 1. Pantallas y rutas

Las rutas MVC son una **propuesta**, siguiendo la convención `/{Controller}/{Acción}/{id?}`.

| Pantalla | Ruta en el prototipo | Componente del prototipo | Ruta propuesta (MVC) | Entrega | Barra inferior |
|---|---|---|---|---|---|
| Registro | — (no existe) | — | `/Cuenta/Registro` | 1ª | No |
| Inicio de sesión | — (no existe) | — | `/Cuenta/Login` | 1ª | No |
| Onboarding (5 pasos) | `/` | `Onboarding.tsx` | `/Perfil/Onboarding` | 1ª | No |
| Home (recetas para el usuario) | `/home` | `Home.tsx` | `/Recetas` | 1ª | Sí |
| Detalle de receta | `/receta/:id` | `RecipeDetail.tsx` | `/Recetas/Detalle/{id}` | 1ª | Sí |
| Mi Despensa | `/despensa` | `Despensa.tsx` | `/Despensa` | 1ª | Sí |
| Tiendas | `/tiendas` | `StoreLocator.tsx` | `/Tiendas` | 1ª (links a Google Maps, D-09) | Sí |
| Perfil (ver y editar) | `/perfil` | `UserProfile.tsx` | `/Perfil` | 1ª | Sí |
| Administración de recetas | — (no existe) | — | `/Admin/Recetas` | 1ª | No |

La barra inferior tiene cuatro accesos: **Inicio** (`/Recetas`), **Despensa**, **Tiendas** y **Perfil**. Va una sola vez en `Views/Shared/_Layout.cshtml` y se oculta en registro, login, onboarding y administración (en el prototipo se oculta solo en el onboarding: `Layout.tsx`).

## 2. Paleta

El prototipo usa la paleta de Tailwind 4.1. Los valores hexadecimales son los de la paleta estándar de Tailwind (Tailwind 4 los define en `oklch`, con diferencias mínimas).

### Colores de marca y de cada pantalla

| Uso | Clases del prototipo | Hex | Variable propuesta |
|---|---|---|---|
| Primario (botones, acentos, ítem activo de la barra) | `orange-500` / hover `orange-600` | `#f97316` / `#ea580c` | `--color-primario` / `--color-primario-oscuro` |
| Fondo general (degradado) | `from-orange-50 via-amber-50 to-yellow-50` | `#fff7ed` → `#fffbeb` → `#fefce8` | `--fondo-app` |
| Fondo ítem activo de la barra, chips seleccionados | `orange-50` | `#fff7ed` | `--color-primario-suave` |
| Header de Home | `from-orange-500 to-amber-500` | `#f97316` → `#f59e0b` | `--degradado-home` |
| Header de Detalle (fondo del plato) | `from-orange-200 to-amber-200` | `#fed7aa` → `#fde68a` | `--degradado-plato` |
| Header de Despensa, botones de Despensa | `from-emerald-500 to-teal-500`, `emerald-500` / `600` | `#10b981` → `#14b8a6`, `#059669` | `--degradado-despensa`, `--color-despensa` |
| Header de Tiendas, botones de Tiendas | `from-green-500 to-emerald-500`, `green-500` / `600` | `#22c55e` → `#10b981`, `#16a34a` | `--degradado-tiendas`, `--color-tiendas` |
| Header de Perfil | `from-purple-500 to-pink-500` | `#a855f7` → `#ec4899` | `--degradado-perfil` |
| Recetas "Casi listas" (borde y porcentaje) | `amber-500`, `amber-600`, `amber-800` | `#f59e0b`, `#d97706`, `#92400e` | `--color-parcial` |

### Neutros

| Uso | Clase | Hex |
|---|---|---|
| Texto principal | `gray-800` | `#1f2937` |
| Texto secundario | `gray-600` / `gray-700` | `#4b5563` / `#374151` |
| Texto terciario, íconos inactivos | `gray-500` / `gray-400` | `#6b7280` / `#9ca3af` |
| Bordes de tarjetas e inputs | `gray-200` | `#e5e7eb` |
| Fondos de chips y botones secundarios | `gray-100` / hover `gray-200` | `#f3f4f6` / `#e5e7eb` |
| Tarjetas | `white` | `#ffffff` |

### Estados

| Estado | Fondo | Texto | Uso |
|---|---|---|---|
| Dificultad fácil | `green-100` `#dcfce7` | `green-700` `#15803d` | Badge de la tarjeta de receta |
| Dificultad media | `yellow-100` `#fef9c3` | `yellow-700` `#a16207` | Badge |
| Dificultad difícil | `red-100` `#fee2e2` | `red-700` `#b91c1c` | Badge |
| Alergia seleccionada | `red-50` `#fef2f2`, borde `red-500` `#ef4444` | `red-900` | Onboarding y perfil |
| Advertencia de alérgenos | `red-50` con borde izquierdo `red-500` | `red-800` `#991b1b` | Detalle de receta |
| Paso completado | `green-50` `#f0fdf4`, borde `green-500` | `green-800` | Detalle, pestaña Pasos |
| Favorito activo | — | `red-500` `#ef4444` (corazón relleno) | Home |

## 3. Tipografía

- **Fuente:** el prototipo no carga fuentes propias (`src/styles/fonts.css` está vacío), así que usa la del sistema. En CSS: `font-family: system-ui, -apple-system, "Segoe UI", Roboto, sans-serif;` (Bootstrap ya trae una pila equivalente).
- **Tamaño base:** 16 px.
- **Títulos** (`theme.css`): `h1` 1.5 rem, `h2` 1.25 rem, `h3` 1.125 rem, `h4` 1 rem; todos con peso 500 y alto de línea 1.5.
- **Texto chico:** 0.875 rem (descripciones, metadatos); 0.75 rem en las etiquetas de la barra inferior.
- Mínimo de 14 px para texto (RNF3 de la documentación funcional).

## 4. Espaciado, bordes y sombras

| Elemento | Prototipo | Valor |
|---|---|---|
| Ancho máximo del contenido | `max-w-lg` | 512 px, centrado |
| Ancho de la tarjeta del onboarding | `max-w-md` | 448 px |
| Padding de pantalla y tarjetas | `p-6` / `px-6`, `p-4` en tarjetas | 24 px / 16 px |
| Separación entre secciones | `mb-6` | 24 px |
| Separación entre elementos | `gap-2` (lo más usado) / `mb-4` | 8 px / 16 px |
| Espacio inferior para no tapar contenido con la barra | `pb-24` | 96 px |
| Radio de botones e inputs | `rounded-lg` | 10 px (`--radius: 0.625rem`) |
| Radio de tarjetas | `rounded-xl` | 14 px |
| Radio del panel del detalle | `rounded-t-3xl` | 24 px arriba |
| Chips, avatar, botones circulares | `rounded-full` | 9999 px |
| Sombra de tarjetas | `shadow-md`, hover `shadow-xl` | sombras de Bootstrap `shadow-sm` / `shadow` |
| Sombra de la barra inferior y headers | `shadow-lg` | `shadow` de Bootstrap |
| Transiciones | `transition-colors`, `transition-all` | 300 ms (RNF6) |

## 5. Componentes que se repiten

Conviene armarlos una vez (vista parcial Razor o clase CSS) y reutilizarlos.

| Componente | Dónde aparece | Descripción | Implementación sugerida |
|---|---|---|---|
| Header con degradado | Home, Despensa, Tiendas, Perfil | Bloque superior con título, subtítulo y el degradado de la pantalla | Clase `.header-pantalla` + modificador de color (`.header-home`, `.header-despensa`…) |
| Barra de navegación inferior | Pantallas del usuario | Fija abajo, fondo blanco, borde superior naranja suave; ícono y etiqueta; activo en naranja con fondo `orange-50` | En `_Layout.cshtml`; el ítem activo según el controller actual |
| Tarjeta de receta | Home, Despensa | Imagen o emoji a la izquierda, nombre, descripción, tiempo, porciones, badge de dificultad y corazón | Vista parcial `_TarjetaReceta.cshtml` |
| Chip seleccionable | Onboarding, Perfil, filtros de Home y Tiendas | Botón redondeado gris; seleccionado en color de la sección | Checkbox o botón con clase `.chip` / `.chip-activo` |
| Chip removible | Personalizadas, despensa | Texto + "×" | `.chip` con botón de cierre |
| Botón primario | Todas | Naranja, texto blanco, `rounded-lg`, hover más oscuro | `.btn` de Bootstrap con color `--color-primario` |
| Badge de dificultad | Tarjeta y detalle | Pastilla verde, amarilla o roja | `.badge-facil`, `.badge-media`, `.badge-dificil` |
| Indicador de pasos | Onboarding | 5 círculos numerados; completados con ✓ en naranja | Lista con estilo propio |
| Banner de alérgenos | Detalle | Fondo rojo claro con borde izquierdo rojo y la lista de alérgenos | `.alert` de Bootstrap con estilo propio |
| Pestañas | Detalle (Ingredientes / Pasos), Despensa (Podés cocinar / Casi listas) | Dos pestañas, la activa subrayada o rellena | `nav-tabs` / `nav-pills` de Bootstrap |
| Barra de progreso | Despensa | Porcentaje de ingredientes disponibles | `.progress` de Bootstrap en color `--color-parcial` |
| Estado vacío | Despensa, Home sin resultados | Ícono grande, título y texto orientativo | Vista parcial `_EstadoVacio.cshtml` |

## 6. Íconos

El prototipo usa Lucide: `Home`, `Refrigerator`, `MapPin`, `User` (barra inferior), `ChefHat`, `Clock`, `Users`, `Flame`, `Heart`, `Search`, `Filter`, `ArrowLeft`, `ArrowRight`, `Check`, `X`, `Plus`, `ShoppingCart`, `AlertCircle`, `Utensils`, `Edit2`, `Navigation`, `Star`.

Decisión pendiente (`CLAUDE.md`, sección 7): Bootstrap Icons (tiene equivalentes de todos: `house`, `basket`, `geo-alt`, `person`, `clock`, `people`, `fire`, `heart`, `search`…), emojis o SVG inline. Las recetas del prototipo usan un emoji como imagen (`🥗`, `🌮`…), que sirve de imagen genérica hasta tener las fotos (2ª entrega).

## 7. Variables CSS propuestas para `wwwroot/css/estilos.css`

```css
:root {
  --color-primario: #f97316;
  --color-primario-oscuro: #ea580c;
  --color-primario-suave: #fff7ed;
  --color-despensa: #10b981;
  --color-tiendas: #22c55e;
  --color-parcial: #f59e0b;
  --color-texto: #1f2937;
  --color-texto-secundario: #4b5563;
  --color-borde: #e5e7eb;

  --fondo-app: linear-gradient(135deg, #fff7ed, #fffbeb, #fefce8);
  --degradado-home: linear-gradient(90deg, #f97316, #f59e0b);
  --degradado-plato: linear-gradient(135deg, #fed7aa, #fde68a);
  --degradado-despensa: linear-gradient(90deg, #10b981, #14b8a6);
  --degradado-tiendas: linear-gradient(90deg, #22c55e, #10b981);
  --degradado-perfil: linear-gradient(90deg, #a855f7, #ec4899);

  --ancho-maximo: 512px;
  --radio: 10px;
  --radio-tarjeta: 14px;
}
```

## 8. Diferencias entre el prototipo y la documentación funcional

El prototipo no tiene registro, login ni administración, y en varios puntos se comporta distinto de lo documentado. El detalle está en `docs/CONFLICTOS.md`. **Manda la documentación funcional**; el prototipo define solo el aspecto visual.
