# CocinaFácil - Documentación de Diseño

## 📱 Descripción General

**CocinaFácil** es una aplicación web móvil diseñada para asistir a los usuarios en la preparación de comidas, ofreciendo una experiencia personalizada según sus preferencias, restricciones alimenticias y alergias.

---

## 🎯 Objetivos de la Aplicación

- Personalizar la experiencia de cocina según el perfil del usuario
- Facilitar la búsqueda de recetas adaptadas a restricciones y alergias
- Estimar tiempos de preparación de manera clara
- Sugerir recetas según los ingredientes disponibles en el hogar
- Indicar qué ingredientes faltantes se deben comprar y dónde
- Ayudar a localizar tiendas cercanas para comprar ingredientes
- Ofrecer una interfaz intuitiva y amigable

---

## 🖼️ Pantallas de la Aplicación

### 1. **Onboarding (Página de Bienvenida)**

**Ruta:** `/`

**Propósito:** Capturar el perfil del usuario, preferencias culinarias, restricciones alimenticias y alergias.

**Características:**

- **Proceso en 5 pasos:**
  1. Captura del nombre del usuario
  2. Selección de restricciones alimenticias (Vegetariano, Vegano, Sin Gluten, etc.) + opción de agregar restricciones personalizadas
  3. Selección de alergias (Frutos Secos, Mariscos, Pescado, etc.) + opción de agregar alergias personalizadas
  4. Selección de gustos culinarios (Italiana, Mexicana, Asiática, etc.)
  5. Nivel de experiencia en cocina (Principiante, Intermedio, Avanzado)

- **Elementos de UI:**
  - Indicador de progreso con 5 círculos
  - Botones de selección múltiple con estados activo/inactivo
  - Campo de texto para agregar restricciones/alergias personalizadas
  - Chips removibles para mostrar opciones personalizadas agregadas
  - Botón "Agregar" para confirmar opciones personalizadas
  - Navegación con botones "Atrás" y "Siguiente"
  - Icono de chef en la parte superior

**Diseño:**
- Fondo degradado en tonos naranjas y amarillos
- Tarjeta blanca central con bordes redondeados
- Color de acento: Naranja (#F97316)
- Botones con feedback visual al seleccionar

---

### 2. **Home / Dashboard (Pantalla Principal)**

**Ruta:** `/home`

**Propósito:** Mostrar recetas personalizadas según el perfil del usuario.

**Características:**

- **Barra de búsqueda** con icono de lupa
- **Sistema de filtros** por categorías (Rápido, Saludable, Vegano, etc.)
- **Lista de recetas** con:
  - Emoji representativo de cada plato
  - Nombre y descripción
  - Tiempo de preparación
  - Número de porciones
  - Nivel de dificultad (Fácil, Media, Difícil)
  - Botón de favoritos (corazón)

- **Personalización automática:**
  - Filtra recetas según alergias del usuario
  - Prioriza recetas según restricciones alimenticias

**Diseño:**
- Header con degradado naranja a ámbar
- Tarjetas blancas con sombra para cada receta
- Sistema de colores para dificultad:
  - Verde: Fácil
  - Amarillo: Media
  - Rojo: Difícil

---

### 3. **Detalle de Receta**

**Ruta:** `/receta/:id`

**Propósito:** Mostrar información completa sobre una receta específica.

**Características:**

- **Header visual** con emoji grande del plato
- **Información nutricional:**
  - Tiempo de preparación
  - Número de porciones
  - Calorías

- **Advertencia de alérgenos** (si aplica) con banner rojo

- **Dos pestañas:**
  1. **Ingredientes:**
     - Lista de ingredientes con cantidades
     - Botón "Ver dónde comprar" que navega a la pantalla de tiendas
  
  2. **Pasos de preparación:**
     - Lista numerada de pasos
     - Checkbox interactivo para marcar pasos completados
     - Animación de celebración al completar todos los pasos

**Diseño:**
- Fondo degradado para la imagen del plato
- Tarjeta con esquinas redondeadas superiores
- Sistema de tabs para alternar entre ingredientes y pasos
- Badges de colores para información nutricional

---

### 4. **Ubicación de Tiendas**

**Ruta:** `/tiendas`

**Propósito:** Mostrar tiendas cercanas donde comprar ingredientes.

**Características:**

- **Mapa visual** con marcadores de ubicación
- **Sistema de filtros** por tipo de tienda:
  - Supermercado
  - Verdulería
  - Carnicería
  - Dietética
  - Especialidades

- **Lista de tiendas** con:
  - Nombre y tipo de tienda
  - Distancia en kilómetros
  - Calificación con estrellas
  - Horario de atención
  - Teléfono
  - Dirección

- **Interactividad:**
  - Click para expandir y ver detalles
  - Botones "Cómo llegar" y "Llamar"

**Diseño:**
- Header con degradado verde esmeralda
- Mapa simulado con marcadores animados
- Tarjetas expandibles para cada tienda
- Badges de distancia con fondo verde

---

### 5. **Mi Despensa (Ingredientes Disponibles)**

**Ruta:** `/despensa`

**Propósito:** Permitir al usuario registrar los ingredientes que tiene en casa para recibir sugerencias de recetas posibles o saber qué debe comprar.

**Características:**

- **Buscador de ingredientes** con sugerencias en tiempo real mientras se escribe
- **Accesos rápidos** a ingredientes comunes para agregar con un solo toque
- **Lista de ingredientes** disponibles con chips removibles y botón de limpiar todo
- **Análisis automático de recetas** con dos secciones:
  1. **"Podés cocinar"** — recetas con el 100% de ingredientes disponibles
  2. **"Casi listas"** — recetas con 50% o más de ingredientes disponibles, ordenadas por porcentaje
- **Barra de progreso** visual para recetas parciales
- **Lista de ingredientes faltantes** con nombre y cantidad para cada receta parcial
- **Acceso directo a Tiendas** desde cada receta para facilitar la compra de lo faltante
- **Navegación a detalle** de receta con click en cualquier resultado

**Diseño:**
- Header con degradado verde esmeralda a teal
- Ícono de refrigerador (Refrigerator) como identificador visual
- Tarjetas con borde izquierdo: verde para completas, ámbar para parciales
- Estado vacío con emoji ilustrativo y texto orientativo

---

### 6. **Perfil de Usuario**

**Ruta:** `/perfil`

**Propósito:** Visualizar y editar las preferencias del usuario.

**Características:**

- **Avatar del usuario** con nombre
- **Modo de edición** activable con botón
- **Secciones:**
  1. Nivel de cocina
  2. Restricciones alimenticias
  3. Alergias
  4. Gustos culinarios

- **Funcionalidad de edición:**
  - Botones de Check (✓) y X para guardar o cancelar
  - Tags interactivos para seleccionar/deseleccionar opciones
  - Campo de texto para agregar restricciones/alergias personalizadas
  - Botón X en chips personalizados para eliminarlos
  - Validación de cambios

**Diseño:**
- Header con degradado púrpura a rosa
- Tarjetas blancas organizadas por categoría
- Sistema de iconos para cada sección:
  - ChefHat: Nivel de cocina
  - Utensils: Restricciones
  - AlertCircle: Alergias
  - Heart: Gustos

---

## 🎨 Sistema de Diseño

### Paleta de Colores

**Colores Principales:**
- Naranja: `#F97316` - Color primario para botones y acciones
- Ámbar: `#F59E0B` - Degradados y highlights
- Verde esmeralda: `#10B981` - Mi Despensa y acciones positivas
- Teal: `#14B8A6` - Degradado de cabecera en Despensa
- Rojo: `#EF4444` - Alergias y advertencias
- Púrpura: `#A855F7` - Perfil de usuario
- Rosa: `#EC4899` - Acentos en perfil

**Colores Neutros:**
- Blanco: `#FFFFFF` - Fondo de tarjetas
- Gris claro: `#F3F4F6` - Fondos secundarios
- Gris oscuro: `#1F2937` - Texto principal

### Tipografía

- **Encabezados (h1):** 2xl (1.5rem), peso medium (500)
- **Subtítulos (h2):** xl (1.25rem), peso medium (500)
- **Texto normal:** base (1rem), peso normal (400)
- **Texto pequeño:** sm (0.875rem), peso normal (400)

### Espaciado

- Padding de contenedor principal: 1.5rem (24px)
- Espaciado entre secciones: 1.5rem (24px)
- Espaciado entre elementos: 1rem (16px)
- Border radius para tarjetas: 0.75rem - 1rem (12px - 16px)

### Iconografía

**Biblioteca:** Lucide React

**Iconos principales:**
- Home: Pantalla de inicio
- Refrigerator: Mi Despensa
- MapPin: Ubicación de tiendas
- User: Perfil de usuario
- ChefHat: Cocina y nivel
- Clock: Tiempo de preparación
- Users: Porciones
- Heart: Favoritos
- Search: Búsqueda
- Filter: Filtros

---

## 🧭 Navegación

### Estructura de Rutas

```
/ (Onboarding)
├── /home (Dashboard)
├── /receta/:id (Detalle de Receta)
├── /despensa (Mi Despensa)
├── /tiendas (Ubicación de Tiendas)
└── /perfil (Perfil de Usuario)
```

### Barra de Navegación Inferior

Disponible en todas las pantallas excepto Onboarding:

- **Inicio** → `/home`
- **Despensa** → `/despensa`
- **Tiendas** → `/tiendas`
- **Perfil** → `/perfil`

**Diseño:**
- Fija en la parte inferior
- Fondo blanco con borde superior
- Iconos con etiquetas
- Estado activo resaltado con color naranja y fondo naranja claro

---

## 🔄 Flujo de Usuario

### Flujo Principal

1. **Primera vez:**
   - Usuario llega a Onboarding
   - Completa 5 pasos de configuración
   - Es redirigido al Home

2. **Exploración de recetas:**
   - Usuario ve recetas personalizadas en Home
   - Puede buscar y filtrar
   - Click en receta → Pantalla de detalle

3. **Preparación de receta:**
   - Usuario ve ingredientes
   - Click en "Ver dónde comprar" → Pantalla de tiendas
   - Regresa a receta
   - Marca pasos completados

4. **Cocinar con lo que hay:**
   - Usuario accede a "Mi Despensa"
   - Agrega ingredientes disponibles en casa
   - Ve recetas posibles de inmediato o las que le faltan pocos ingredientes
   - Desde los ingredientes faltantes puede ir directo a la pantalla de Tiendas

5. **Gestión de perfil:**
   - Usuario accede a Perfil
   - Edita preferencias
   - Cambios se reflejan en recomendaciones

---

## 💡 Características de Personalización

### Sistema de Filtrado Inteligente

**Por Alergias:**
- La app automáticamente excluye recetas que contengan alérgenos declarados por el usuario
- Muestra advertencia visual en recetas con alérgenos
- El usuario puede agregar alergias personalizadas más allá de las opciones predefinidas

**Por Restricciones:**
- Prioriza recetas que cumplan con restricciones alimenticias del usuario
- Vegetarianos no ven recetas con carne
- Usuarios sin gluten ven solo recetas aptas
- Permite agregar restricciones personalizadas (ej: "Sin azúcar", "Bajo en sodio")

**Por Gustos:**
- Ordena recetas según las preferencias culinarias declaradas
- Sugiere recetas de categorías favoritas

### Restricciones y Alergias Personalizadas

**Funcionalidad:**
- Además de las opciones predefinidas, los usuarios pueden agregar sus propias restricciones y alergias
- Campo de texto libre para ingresar restricciones específicas
- Los valores personalizados se muestran como chips removibles
- Disponible tanto en el Onboarding como en la edición de Perfil
- Validación para evitar duplicados y entradas vacías
- Soporte para tecla Enter para agregar rápidamente

**Ejemplos de uso:**
- Restricciones: "Sin azúcar", "Bajo en sodio", "Dieta FODMAP", "Paleo"
- Alergias: "Kiwi", "Apio", "Mostaza", "Semillas de sésamo"

**Beneficios:**
- Mayor flexibilidad para casos particulares
- Adaptación a dietas específicas no contempladas en opciones predefinidas
- Registro preciso de alergias poco comunes

---

## 📊 Datos de Muestra

### Recetas Incluidas (6 recetas)

1. **Ensalada César Vegana** - 20 min, fácil
2. **Tacos de Pescado** - 30 min, media
3. **Pasta Primavera sin Gluten** - 25 min, fácil
4. **Bowl de Quinoa y Garbanzos** - 35 min, fácil
5. **Curry Tailandés de Pollo** - 40 min, media
6. **Smoothie Bowl Tropical** - 10 min, fácil

### Tiendas Incluidas (5 tiendas)

1. Supermercado Fresh Market - 0.5 km
2. Verdulería Don José - 0.8 km
3. Carnicería La Preferida - 1.2 km
4. Dietética Natural Life - 1.5 km
5. Almacén Oriental - 2.0 km

---

## 🛠️ Tecnologías Utilizadas

### Frontend
- **React 18.3.1** - Biblioteca principal
- **TypeScript** - Tipado estático
- **React Router 7.13.0** - Navegación entre páginas
- **Tailwind CSS 4.1.12** - Estilos y diseño responsivo
- **Lucide React** - Biblioteca de iconos

### Gestión de Estado
- **Context API** - Gestión del estado del usuario
- **useState** - Estado local de componentes

---

## 📱 Diseño Responsivo

### Breakpoints

- **Mobile First:** Diseñado primero para móvil
- **Max Width:** 512px (lg) para contenido centrado
- **Navegación:** Fixed bottom navigation bar

### Adaptabilidad

- Tarjetas se adaptan al ancho de la pantalla
- Textos responsivos
- Imágenes escalables
- Touch-friendly (botones grandes, fáciles de presionar)

---

## ♿ Accesibilidad

### Consideraciones

- Contraste de colores adecuado (WCAG 2.1)
- Textos legibles (mínimo 14px)
- Botones con área de click grande (44px mínimo)
- Feedback visual en todas las interacciones
- Navegación por teclado habilitada
- Estados hover/focus visibles

---

## 🎨 Elementos Visuales Destacados

### Gradientes

- **Header Home:** `from-orange-500 to-amber-500`
- **Header Tiendas:** `from-green-500 to-emerald-500`
- **Header Perfil:** `from-purple-500 to-pink-500`
- **Fondo General:** `from-orange-50 via-amber-50 to-yellow-50`

### Sombras

- **Tarjetas:** `shadow-md` → `shadow-xl` en hover
- **Navegación:** `shadow-lg`
- **Botones flotantes:** `shadow-lg`

### Animaciones

- Transitions suaves en botones (300ms)
- Pulse en marcadores de mapa
- Escalado en hover de tarjetas
- Celebración al completar receta

---

## 🚀 Funcionalidades Futuras

### Mejoras Propuestas

1. **Integración con APIs reales:**
   - Google Maps para ubicaciones reales
   - API de supermercados para disponibilidad de productos
   
2. **Funcionalidades sociales:**
   - Compartir recetas
   - Calificar y comentar
   - Lista de compras compartida

3. **Planificación:**
   - Calendario de comidas semanales
   - Generación automática de lista de compras
   - Notificaciones de recordatorio

4. **Inteligencia:**
   - Recomendaciones basadas en IA
   - Sustituciones automáticas de ingredientes
   - Análisis nutricional completo

---

## 📝 Conclusión

**CocinaFácil** es una aplicación completa que aborda las necesidades de usuarios con diferentes niveles de experiencia en cocina, ofreciendo una experiencia personalizada y accesible. El diseño prioriza la usabilidad, la claridad visual y la personalización, haciendo que cocinar sea una experiencia más fácil y agradable.

### Puntos Clave del Diseño

✅ **Personalización total** según perfil del usuario  
✅ **5 pantallas completas** con funcionalidad real  
✅ **Sistema de navegación intuitivo**  
✅ **Diseño moderno y colorido**  
✅ **Filtrado inteligente** por alergias y restricciones  
✅ **Restricciones y alergias personalizadas** definidas por el usuario  
✅ **Información clara** de tiempos y nutrición  
✅ **Integración de ubicación** para compras  

---

**Desarrollado para:** Carrera de Analista en Sistemas  
**Fecha:** Junio 2026  
**Tecnología:** React + TypeScript + Tailwind CSS
