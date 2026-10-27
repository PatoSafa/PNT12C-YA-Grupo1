# Diseño (destilado del prototipo de Figma)

Pantallas, rutas, colores y componentes del prototipo de Figma Make, para reproducirlos con Razor + Bootstrap + `wwwroot/css/estilos.css`.

> El prototipo es React + Tailwind y **quedó desactualizado**: se usa solo como guía visual y de flujo, y el código no se reutiliza. **Si algo choca con la documentación funcional, manda la documentación funcional** (D-03 en `DECISIONES.md`). Las clases Tailwind se citan para poder volver a la fuente.
>
> Ajustes ya decididos sobre el prototipo (D-03):
> - Nombre de la app: **CenaAPP**.
> - Registro, Inicio de sesión, Administración y "Cerrar sesión" se arman con la misma estructura visual que el resto (header con degradado, tarjetas blancas, mismos botones e inputs).
> - Las opciones de restricciones, alergias y gustos salen de los catálogos de la doc funcional (16 / 13 / 12), iguales en Onboarding y Perfil.
> - "Casi listas" muestra recetas con **≥ 50 %** de los ingredientes.
> - Los alérgenos de cada receta se calculan desde sus ingredientes.
> - Favoritos guardados en la base.
> - Tiendas sin teléfono ni rating. El mapa está pospuesto.
> - Frontend lo más simple posible: Razor + Bootstrap, JavaScript solo donde haga falta.

## 1. Pantallas y rutas

| Pantalla | Ruta del prototipo | Estado |
|---|---|---|
| Onboarding (5 pasos) | `/` | Prototipada |
| Inicio (recetas) | `/home` | Prototipada |
| Detalle de receta | `/receta/:id` | Prototipada |
| Mi Despensa | `/despensa` | Prototipada |
| Tiendas cercanas | `/tiendas` | Prototipada |
| Mi Perfil | `/perfil` | Prototipada |
| Registro, Inicio de sesión | — | Solo en la doc funcional; se arman con la estructura de las demás |
| Administración (recetas e ingredientes) | — | Solo en la doc funcional; se arma con la estructura de las demás |

**Flujo:** Onboarding → "Comenzar" → Inicio → tarjeta → Detalle (← vuelve a Inicio; "Ver dónde comprar" → Tiendas). Despensa → tarjeta → Detalle; "Ver tiendas" → Tiendas. La barra inferior conecta Inicio / Despensa / Tiendas / Perfil en todas las pantallas menos Onboarding.

### 1.1 Layout general
- Fondo de página: degradado `orange-50 → amber-50 → yellow-50` (diagonal).
- **Barra de navegación inferior** (una sola vez, en `_Layout.cshtml`):
  - Fija abajo, fondo blanco, borde superior `orange-200`, sombra grande.
  - Fila interna centrada a 512 px, `py-3`, ítems repartidos.
  - 4 ítems: Inicio (casa), Despensa (heladera), Tiendas (pin), Perfil (usuario). Ícono 24 px + texto chico (12 px).
  - Activo: texto `orange-600` sobre `orange-50`. Inactivo: `gray-600`, hover `orange-500`.
  - En el Detalle de receta ningún ítem queda activo.

### 1.2 Onboarding (`/`)
- Centrado vertical, contenedor de 448 px (`max-w-md`).
- Logo: círculo naranja 80 px con gorro de chef blanco. Título "CenaAPP" (en el prototipo dice "CenAPP"), subtítulo "Tu asistente personal de cocina".
- Tarjeta blanca `rounded-2xl`, sombra XL, padding 32 px.
- Indicador de pasos: 5 círculos de 40 px; hechos/actual naranja (los completos con ✓), pendientes gris.
- Pasos:
  1. "¿Cómo te llamas?" — input "Tu nombre".
  2. "¿Tienes restricciones alimenticias?" — opciones Vegetariano, Vegano, Sin Gluten, Sin Lactosa, Kosher, Halal, Bajo en Carbohidratos (seleccionada en **naranja**). Agregar personalizada: "Agregar otra restricción:", placeholder "Ej: Sin azúcar, Bajo en sodio...", botón "Agregar", chips "Restricciones personalizadas:".
  3. "¿Tienes alguna alergia?" — Frutos Secos, Mariscos, Pescado, Huevo, Lácteos, Soja, Gluten, Sésamo (en **rojo**). Placeholder "Ej: Kiwi, Apio, Mostaza...", "Alergias personalizadas:".
  4. "¿Qué tipo de cocina te gusta?" — Italiana, Mexicana, Asiática, Mediterránea, Vegana, Comfort Food, Saludable, Rápido (en **verde**).
  5. "¿Cuál es tu nivel de cocina?" — Principiante "Recién empiezo", Intermedio "Tengo experiencia básica" (por defecto), Avanzado "Soy experimentado".
- Botones: "Atrás" (borde naranja) y "Siguiente" / "Comenzar" → (naranja lleno; deshabilitado en gris si falta el nombre).

### 1.3 Inicio (`/home`)
- Header degradado `orange-500 → amber-500`, texto blanco: "Hola, {nombre}! 👋", "¿Qué vamos a cocinar hoy?", buscador con lupa "Buscar recetas...".
- Tarjeta de filtros (superpuesta al header): "Filtros" + chips Rápido, Saludable, Vegano, Asiática, Italiana, Mexicana (uno activo a la vez; volver a tocar lo desactiva).
- Título "Recetas para ti" (o "Recetas {filtro}") + "{n} recetas".
- Lista de **tarjetas de receta** (ver componentes).
- Vacío: "No se encontraron recetas con estos criterios" + botón "Limpiar filtros".

### 1.4 Detalle de receta (`/receta/:id`)
- Hero con degradado `orange-200 → amber-200` y la imagen (en el prototipo, un emoji grande) de 256 px de alto; botón circular blanco "←" arriba a la izquierda.
- Hoja blanca con esquinas superiores muy redondeadas que sube sobre el hero: nombre, descripción.
- Pills de datos: ⏱ "{n} min" (`orange-50`), 👥 "{n} porciones" (`blue-50`), 🔥 "{n} kcal" (`red-50`).
- Alerta de alérgenos: fondo `red-50`, borde izquierdo rojo de 4 px, "⚠️ Contiene alérgenos:" + lista.
- Pestañas subrayadas "Ingredientes" | "Pasos" (activa en `orange-600`).
  - Ingredientes: filas gris claro con nombre y cantidad; botón verde "🛒 Ver dónde comprar" → Tiendas.
  - Pasos: tarjetas clickeables numeradas; al marcar quedan verdes y tachadas. Con todos hechos: caja verde "🎉 ¡Felicitaciones! Has completado la receta".
- No encontrada: "Receta no encontrada" + "Volver al inicio".

### 1.5 Mi Despensa (`/despensa`)
- Header degradado `emerald-500 → teal-500`, ícono de heladera, "Mi Despensa", "Ingresá los ingredientes que tenés en casa y te sugerimos recetas".
- Tarjeta "Agregar ingrediente": input con lupa "Escribí o elegí de la lista..." y desplegable de sugerencias; botón verde "+".
- Tarjeta "Tengo en casa ({n})": chips verdes con ×, enlace rojo "Limpiar todo".
- Pestañas segmentadas: "Podés cocinar ({n})" (verde) | "Casi listas ({n})" (ámbar).
  - Podés cocinar: tarjeta con borde izquierdo verde, "Tenés todos los ingredientes", tiempo, dificultad, →.
  - Casi listas: borde izquierdo ámbar, barra de progreso ámbar→naranja con %, caja "Necesitás comprar ({n}):" con etiquetas de faltantes y "Ver tiendas →".
- Vacíos: "🥦 ¿Qué tenés en casa?…", "Ninguna receta se puede hacer con estos ingredientes solos.", "No hay recetas con al menos el 50% de ingredientes disponibles."

### 1.6 Tiendas cercanas (`/tiendas`)
- Header degradado `green-500 → emerald-500`, "Tiendas Cercanas", "Encontrá dónde comprar tus ingredientes".
- Tarjeta "Mostrando tiendas cerca de vos" + chips de tipo: Todas, Supermercado, Verdulería, Carnicería, Dietética, Especialidades.
- Mapa de 300 px de alto, redondeado, con pines verdes (el seleccionado más grande).
- Tarjetas de tienda: emoji, nombre, tipo, badge de distancia "{x} km". Al seleccionar se expande: dirección, horario, botón "Cómo llegar" (verde). El prototipo además muestra rating, teléfono y "Llamar": **no van** (la doc funcional no los incluye).
- Vacío: "No se encontraron tiendas de este tipo" + "Ver todas las tiendas".

### 1.7 Mi Perfil (`/perfil`)
- Header degradado `purple-500 → pink-500`: "Mi Perfil", botón "Editar" translúcido (en edición: ✓ verde y ✗ rojo), avatar circular, nombre, "Nivel: {nivel}".
- Cuatro tarjetas: Nivel de Cocina (violeta), Restricciones Alimenticias (naranja), Alergias (rojo), Gustos Culinarios (rosa). En vista: chips o "Ninguna" / "No especificado". En edición: chips seleccionables + agregar personalizadas.
- Caja "💡 Consejo — Mantén tu perfil actualizado para recibir las mejores recomendaciones…".
- Falta en el prototipo: botón "Cerrar sesión" (lo pide la doc funcional). Va al final del perfil, con estilo de botón de contorno rojo.

## 2. Paleta

El prototipo usa los colores estándar de Tailwind. Propuesta de variables para `estilos.css` (nombres a definir al implementar):

| Rol | Color Tailwind | Hex |
|---|---|---|
| Primario (marca) | orange-500 / hover orange-600 | `#F97316` / `#EA580C` |
| Primario claro | orange-50 / 100 / 200 / 300 | `#FFF7ED` / `#FFEDD5` / `#FED7AA` / `#FDBA74` |
| Primario oscuro (texto) | orange-800 / 900 | `#9A3412` / `#7C2D12` |
| Secundario | amber-500 | `#F59E0B` |
| Ámbar (Casi listas) | amber-50 / 100 / 400 / 600 / 800 | `#FFFBEB` / `#FEF3C7` / `#FBBF24` / `#D97706` / `#92400E` |
| Éxito / tiendas | green-500 / 600 | `#22C55E` / `#16A34A` |
| Éxito claro / texto | green-50 / 100 / 700 / 800 | `#F0FDF4` / `#DCFCE7` / `#15803D` / `#166534` |
| Despensa | emerald-500 / 600, teal-500 | `#10B981` / `#059669`, `#14B8A6` |
| Despensa claro / texto | emerald-100 / 800 | `#D1FAE5` / `#065F46` |
| Peligro / alergias | red-500 / 600 | `#EF4444` / `#DC2626` |
| Peligro claro / texto | red-50 / 100 / 700 / 800 | `#FEF2F2` / `#FEE2E2` / `#B91C1C` / `#991B1B` |
| Perfil | purple-500 / 600, pink-500 / 600 | `#A855F7` / `#9333EA`, `#EC4899` / `#DB2777` |
| Info (porciones) | blue-50 / 600 | `#EFF6FF` / `#2563EB` |
| Dificultad media / rating | yellow-100 / 700, yellow-500 | `#FEF9C3` / `#A16207`, `#EAB308` |
| Texto principal | gray-800 | `#1F2937` |
| Texto secundario | gray-600 / 700 | `#4B5563` / `#374151` |
| Texto apagado | gray-500 / 400 | `#6B7280` / `#9CA3AF` |
| Superficies y bordes | gray-50 / 100 / 200 / 300 | `#F9FAFB` / `#F3F4F6` / `#E5E7EB` / `#D1D5DB` |
| Tarjetas | blanco | `#FFFFFF` |
| Fondo de página | orange-50 → amber-50 → yellow-50 | `#FFF7ED` → `#FFFBEB` → `#FEFCE8` |

**Degradado del header por sección**

| Sección | Desde | Hasta |
|---|---|---|
| Inicio | orange-500 `#F97316` | amber-500 `#F59E0B` |
| Despensa | emerald-500 `#10B981` | teal-500 `#14B8A6` |
| Tiendas | green-500 `#22C55E` | emerald-500 `#10B981` |
| Perfil | purple-500 `#A855F7` | pink-500 `#EC4899` |
| Hero de receta | orange-200 `#FED7AA` | amber-200 `#FDE68A` |

**Badges de dificultad:** fácil `green-100`/`green-700`, media `yellow-100`/`yellow-700`, difícil `red-100`/`red-700`.

**Color por tipo de preferencia** (se repite en Onboarding y Perfil): restricciones naranja, alergias rojo, gustos verde (Onboarding) / rosa (Perfil), nivel violeta (Perfil).

## 3. Tipografía
- Sin fuente propia: la pila del sistema (`system-ui, sans-serif`), que también es la de Bootstrap.
- Base 16 px. Peso máximo usado: 500 (medium).

| Uso | Tamaño |
|---|---|
| Nombre de la app (Onboarding) | 30 px (`text-3xl`) |
| Título de página (h1), nombre en perfil | 24 px (`text-2xl`) |
| Título de sección, pregunta del onboarding | 20 px (`text-xl`) |
| Título de tarjeta | 16 px, peso 500 |
| Cuerpo | 16 px |
| Descripciones, chips | 14 px (`text-sm`) |
| Metadatos, etiquetas de la barra, badges | 12 px (`text-xs`) — ojo con RNF3 (texto ≥ 14 px) |

## 4. Espaciado, radios, sombras
- **Contenedor:** 512 px centrado (`max-w-lg`) en todas las pantallas (Onboarding: 448 px). Coincide con RNF1.
- **Header:** padding 24 px (32 px abajo). Cuerpo: padding lateral 24 px (16 px en Despensa y Tiendas).
- **Superposición:** la primera tarjeta sube 16 px sobre el header (`-mt-4`).
- **Espacio para la barra inferior:** padding inferior de 96 px (`pb-24`; Despensa 112 px).
- **Separación:** entre tarjetas 12–24 px; dentro de tarjetas 16 px (listas) o 24 px (perfil); entre chips 8 px.
- **Radios:** 8 px inputs/botones/filas; 12 px tarjetas, buscador, mapa; 16 px tarjeta de Onboarding; 24 px hoja del detalle; píldora para chips, badges, avatares y círculos de paso; 6 px etiquetas de faltantes.
- **Sombras:** media en tarjetas de lista (grande en hover); grande en tarjetas de sección, barra y botón volver; extra grande en Onboarding, hoja del detalle y desplegables.
- **Transiciones:** cambios de color y sombra (RNF6 pide ~300 ms).

## 5. Componentes que se repiten

| # | Componente | Dónde aparece | Cómo se ve | Base sugerida en Bootstrap |
|---|---|---|---|---|
| 1 | Header con degradado | Inicio, Despensa, Tiendas, Perfil | Degradado de la sección, h1 blanco + subtítulo claro | Clase propia `.encabezado` con variable de color por sección |
| 2 | Tarjeta blanca superpuesta | Todas | Blanco, radio 12 px, sombra, padding 16–24 px, `-mt-4` | `.card` + clase propia |
| 3 | Tarjeta de receta | Inicio (grande), Despensa (compacta, borde izq. de color) | Imagen cuadrada a la izquierda, nombre, ❤, descripción 2 líneas, ⏱ min, 👥 porciones, badge dificultad | `.card` horizontal (`.d-flex`) — vista parcial Razor reutilizable |
| 4 | Chip de filtro | Inicio, Tiendas, Perfil (edición) | Píldora 14 px; activo color lleno, inactivo gris | `.btn .rounded-pill` o `.badge` + clase propia |
| 5 | Chip removible | Onboarding, Perfil, Despensa | Píldora color claro con × | `.badge .rounded-pill` + botón cerrar |
| 6 | Opción seleccionable de ancho completo | Onboarding, Perfil (nivel) | Borde 2 px gris; seleccionada borde y fondo del color | `.btn-check` + `label.btn` o `.list-group-item` |
| 7 | Botones | Todas | Primario naranja lleno; contorno con borde 2 px; éxito verde; peligro rojo; translúcido sobre header | `.btn` con colores pisados por variables |
| 8 | Input | Onboarding, Inicio, Despensa, Perfil | Borde 2 px gris, foco naranja; con ícono a la izquierda | `.form-control` (+ `.input-group`) |
| 9 | Pestañas | Detalle (subrayadas), Despensa (segmentadas) | Ver pantallas | `.nav-tabs` / `.btn-group` |
| 10 | Pill de dato | Detalle | Ícono + texto sobre fondo claro | `.badge` + clase propia |
| 11 | Alerta | Detalle (alérgenos) | Fondo rojo claro, borde izquierdo rojo 4 px | `.alert .alert-danger` + borde izquierdo propio |
| 12 | Estado vacío | Inicio, Despensa, Tiendas | Ícono/emoji grande gris, texto gris, botón opcional | Clase propia `.vacio` |
| 13 | Barra de progreso | Despensa | 8 px, fondo gris, relleno degradado ámbar→naranja | `.progress` + `.progress-bar` con degradado propio |

No hay modales ni toasts en el prototipo. La doc funcional pide confirmación antes de eliminar (admin).

## 6. Íconos
El prototipo usa **lucide**. Íconos que aparecen: casa, heladera, pin de mapa, usuario, gorro de chef, flecha derecha/izquierda, check, lupa, reloj, usuarios, filtro, corazón, llama, carrito, más, cruz, check en círculo, alerta en círculo, chevron abajo, navegación, estrella, teléfono, lápiz, cubiertos.

Alternativa (decisión pendiente): Bootstrap Icons (casi todos tienen equivalente: `house`, `geo-alt`, `person`, `clock`, `people`, `funnel`, `heart`, `fire`, `cart`, `plus`, `x`, `check-circle`, `exclamation-circle`, `star`, `telephone`, `pencil`…), emojis, o SVG inline.

## 7. Datos de ejemplo del prototipo (referencia)
- **Receta:** id, nombre, descripción, imagen (emoji), tiempo de preparación (min), porciones, dificultad (fácil / media / difícil), categorías, restricciones, alérgenos, ingredientes (nombre + cantidad en texto), pasos (texto), calorías.
  - 6 recetas: Ensalada César Vegana, Tacos de Pescado, Pasta Primavera sin Gluten, Bowl de Quinoa y Garbanzos, Curry Tailandés de Pollo, Smoothie Bowl Tropical.
- **Tienda:** id, nombre, tipo, distancia (km), dirección, horario, rating, teléfono, coordenadas, emoji. 5 tiendas de ejemplo en CABA; mapa centrado en (-34.6067, -58.3800).
- **Perfil:** nombre, restricciones, alergias, gustos, nivel de cocina.
- **Despensa:** lista de nombres de ingredientes; 20 sugerencias comunes (Huevos, Leche, Mantequilla, Aceite de oliva, Ajo, Cebolla, Tomate, Papa, Zanahoria, Sal, Pimienta, Harina, Arroz, Pasta, Lenteja, Pollo, Queso, Limón, Espinaca, Pimiento).

La doc funcional (sección 9) define el modelo de datos real; estos datos solo sirven para ver cómo se muestra cada cosa.
