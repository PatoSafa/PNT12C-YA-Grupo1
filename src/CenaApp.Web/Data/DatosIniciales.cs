// Carga los datos iniciales (catálogos, ingredientes, recetas y usuarios de prueba) si la base está vacía.
// Recetas tomadas de docs/funcional/guia_100_recetas_restricciones.pdf (cantidades por porción).
// Tiempos, dificultad, calorías y pasos son un BORRADOR para que el equipo revise (D-04).
using CenaApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace CenaApp.Web.Data;

public static class DatosIniciales
{
    public static void Cargar(AppDbContext db, IConfiguration config)
    {
        if (!db.Categorias.Any())
        {
            CargarCatalogo(db);
        }
        if (!db.Usuarios.Any())
        {
            CargarUsuarios(db, config);
        }
    }

    private static void CargarCatalogo(AppDbContext db)
    {
        // ---------- Catálogos (D-04: 5 restricciones y 5 alérgenos; gustos y categorías de la doc funcional) ----------
        var restricciones = new[]
        {
            new Restriccion { Nombre = "Vegetariana", Descripcion = "Sin carnes de ningún tipo (vaca, pollo, cerdo, pescado)." },
            new Restriccion { Nombre = "Vegana", Descripcion = "Sin carnes, lácteos, huevo, miel ni otros productos animales." },
            new Restriccion { Nombre = "Sin gluten", Descripcion = "Sin trigo, avena común, cebada ni centeno (apto celíacos)." },
            new Restriccion { Nombre = "Sin lactosa", Descripcion = "Sin leche, yogur, crema ni quesos." },
            new Restriccion { Nombre = "Baja en sodio", Descripcion = "Sin sal agregada, salsa de soja, embutidos ni quesos salados." },
        }.ToDictionary(r => r.Nombre);

        var alergenos = new[] { "Gluten", "Lácteos", "Huevo", "Frutos secos", "Pescado y mariscos" }
            .Select(n => new Alergeno { Nombre = n })
            .ToDictionary(a => a.Nombre);

        var gustos = new[] { "Italiana", "Mexicana", "Asiática", "Mediterránea", "Argentina", "Latinoamericana", "Medio Oriente", "India" }
            .Select(n => new Gusto { Nombre = n, Tipo = TipoGusto.Cocina })
            .Concat(new[] { "Comfort Food", "Saludable", "Vegana", "Rápido" }
                .Select(n => new Gusto { Nombre = n, Tipo = TipoGusto.Estilo }))
            .ToDictionary(g => g.Nombre);

        var categorias = new[] { "Plato principal", "Ensalada", "Sopa o guiso", "Acompañamiento", "Desayuno y merienda", "Panificados y snacks", "Postre" }
            .Select(n => new Categoria { Nombre = n })
            .ToDictionary(c => c.Nombre);

        // ---------- Ingredientes ----------
        // alergenos / incompatibles: nombres separados por coma, tal como están en los catálogos de arriba.
        var ingredientes = new Dictionary<string, Ingrediente>();

        void Ing(string nombre, string grupo, string? sinonimos = null,
            string alergenosDe = "", string incompatibles = "", bool cuentaEnDespensa = true)
        {
            ingredientes[nombre] = new Ingrediente
            {
                Nombre = nombre,
                Grupo = grupo,
                Sinonimos = sinonimos,
                CuentaEnDespensa = cuentaEnDespensa,
                Alergenos = Lista(alergenosDe).Select(a => alergenos[a]).ToList(),
                RestriccionesIncompatibles = Lista(incompatibles).Select(r => restricciones[r]).ToList(),
            };
        }

        const string carne = "Vegetariana, Vegana";
        const string lacteo = "Vegana, Sin lactosa";

        // Verduras
        Ing("Papa", "Verdura", "patata");
        Ing("Cebolla", "Verdura", "cebolla morada");
        Ing("Cebolla de verdeo", "Verdura", "verdeo");
        Ing("Pimiento", "Verdura", "morrón, ají");
        Ing("Zanahoria", "Verdura");
        Ing("Zucchini", "Verdura", "zapallito, calabacín");
        Ing("Calabaza", "Verdura", "zapallo, calabaza anco");
        Ing("Champiñones", "Verdura", "hongos, portobello");
        Ing("Puerro", "Verdura");
        Ing("Tomate", "Verdura", "tomate cherry, tomate perita");
        Ing("Salsa de tomate", "Verdura", "tomate triturado, puré de tomate");
        Ing("Pepino", "Verdura");
        Ing("Espinaca", "Verdura");
        Ing("Espárragos", "Verdura");
        Ing("Apio", "Verdura");
        Ing("Lechuga", "Verdura");
        Ing("Berenjena", "Verdura");
        Ing("Choclo", "Verdura", "maíz dulce, elote");
        Ing("Ajo", "Verdura", "diente de ajo");
        Ing("Perejil", "Verdura");
        Ing("Cilantro", "Verdura", "coriandro");
        Ing("Menta", "Verdura", "hierbabuena");

        // Frutas
        Ing("Banana", "Fruta", "plátano");
        Ing("Limón", "Fruta", "lima");
        Ing("Palta", "Fruta", "aguacate");
        Ing("Manzana", "Fruta", "manzana verde");
        Ing("Frutos rojos", "Fruta", "frutillas, fresas, arándanos");
        Ing("Pera", "Fruta");
        Ing("Naranja", "Fruta");

        // Carnes y pescados
        Ing("Pechuga de pollo", "Carne", "pollo, suprema", incompatibles: carne);
        Ing("Carne vacuna", "Carne", "carne picada, lomo, cuadril, nalga", incompatibles: carne);
        Ing("Merluza", "Pescado", "filet de merluza", "Pescado y mariscos", carne);
        Ing("Salmón", "Pescado", alergenosDe: "Pescado y mariscos", incompatibles: carne);

        // Huevo y lácteos
        Ing("Huevo", "Huevo", "claras, yema", "Huevo", "Vegana");
        Ing("Leche", "Lácteo", "leche descremada", "Lácteos", lacteo);
        Ing("Yogur natural", "Lácteo", "yogur", "Lácteos", lacteo);
        Ing("Queso fresco", "Lácteo", "queso cremoso", "Lácteos", lacteo);
        Ing("Queso muzzarella", "Lácteo", "mozzarella", "Lácteos", lacteo);
        Ing("Queso parmesano", "Lácteo", "queso rallado", "Lácteos", lacteo + ", Baja en sodio");

        // Cereales y harinas
        Ing("Arroz", "Cereal y harina", "arroz arborio, arroz basmati, arroz blanco");
        Ing("Arroz integral", "Cereal y harina");
        Ing("Harina de arroz", "Cereal y harina");
        Ing("Almidón de maíz", "Cereal y harina", "maicena, fécula de maíz");
        Ing("Harina de maíz precocida", "Cereal y harina", "harina para arepas");
        Ing("Fécula de mandioca", "Cereal y harina", "almidón de mandioca, tapioca");
        Ing("Fideos de arroz", "Cereal y harina");
        Ing("Quínoa", "Cereal y harina", "quinoa");
        Ing("Avena sin gluten", "Cereal y harina", "avena certificada");
        Ing("Avena", "Cereal y harina", "copos de avena, avena instantánea", "Gluten", "Sin gluten");
        Ing("Tortilla de trigo", "Cereal y harina", "wrap, tortilla", "Gluten", "Sin gluten");
        Ing("Pan rallado", "Cereal y harina", "rebozador", "Gluten", "Sin gluten");

        // Legumbres
        Ing("Garbanzos", "Legumbre", "garbanzo");
        Ing("Harina de garbanzos", "Legumbre");
        Ing("Lentejas", "Legumbre", "lentejas rojas, lentejas turcas");
        Ing("Porotos", "Legumbre", "frijoles, alubias, porotos negros");
        Ing("Tofu", "Legumbre", "queso de soja");

        // Frutos secos y semillas
        Ing("Almendras", "Fruto seco y semilla", alergenosDe: "Frutos secos");
        Ing("Nueces", "Fruto seco y semilla", alergenosDe: "Frutos secos");
        Ing("Semillas de lino", "Fruto seco y semilla", "linaza");
        Ing("Tahini", "Fruto seco y semilla", "pasta de sésamo");

        // Otros
        Ing("Leche de coco", "Otro");
        Ing("Leche de almendras", "Otro", "bebida de almendras", "Frutos secos");
        Ing("Caldo de verduras", "Otro", "caldo casero");
        Ing("Aceitunas", "Otro", incompatibles: "Baja en sodio");
        Ing("Levadura nutricional", "Otro");
        Ing("Cacao amargo", "Otro", "cacao");
        Ing("Jarabe de agave", "Otro", "agave");
        // Salsa de soja común: contiene trigo. Se marca con gluten por precaución.
        Ing("Salsa de soja", "Condimento", "salsa de soya", "Gluten", "Sin gluten, Baja en sodio");
        Ing("Mostaza", "Condimento", incompatibles: "Baja en sodio");

        // Básicos que no cuentan en el porcentaje de la despensa (VN-10)
        Ing("Agua", "Básico", cuentaEnDespensa: false);
        Ing("Aceite", "Básico", "aceite de girasol", cuentaEnDespensa: false);
        Ing("Aceite de oliva", "Básico", "oliva", cuentaEnDespensa: false);
        Ing("Edulcorante", "Básico", cuentaEnDespensa: false);
        Ing("Nuez moscada", "Básico", cuentaEnDespensa: false);
        Ing("Comino", "Básico", cuentaEnDespensa: false);
        Ing("Pimentón", "Básico", "pimentón dulce, pimentón ahumado", cuentaEnDespensa: false);
        Ing("Curry en polvo", "Básico", "curry", cuentaEnDespensa: false);
        Ing("Cúrcuma", "Básico", cuentaEnDespensa: false);
        Ing("Canela", "Básico", cuentaEnDespensa: false);
        Ing("Clavo de olor", "Básico", cuentaEnDespensa: false);
        Ing("Esencia de vainilla", "Básico", "vainilla", cuentaEnDespensa: false);

        // ---------- Recetas ----------
        var recetas = new List<Receta>();

        void Rec(string nombre, string emoji, string categoria, string gustosDe, int tiempoMin, Dificultad dificultad,
            int calorias, string descripcion, (string Nombre, decimal? Cantidad, string? Unidad)[] usa, params string[] pasos)
        {
            recetas.Add(new Receta
            {
                Nombre = nombre,
                Emoji = emoji,
                Descripcion = descripcion,
                Categoria = categorias[categoria],
                Gustos = Lista(gustosDe).Select(g => gustos[g]).ToList(),
                TiempoMin = tiempoMin,
                Porciones = 1, // la guía da las cantidades para una porción
                Dificultad = dificultad,
                Calorias = calorias,
                Ingredientes = usa.Select(u => new RecetaIngrediente
                {
                    Ingrediente = ingredientes[u.Nombre],
                    Cantidad = u.Cantidad,
                    Unidad = u.Unidad,
                }).ToList(),
                Pasos = pasos.Select((texto, i) => new Paso { Orden = i + 1, Texto = texto }).ToList(),
            });
        }

        // Cantidad null = "a gusto"
        Rec("Ñoquis de papa artesanales", "🥔", "Plato principal", "Italiana, Argentina, Comfort Food", 60, Dificultad.Dificil, 600,
            "Ñoquis caseros sin TACC, hechos con harina de arroz.",
            [("Papa", 200, "g"), ("Harina de arroz", 70, "g"), ("Almidón de maíz", 30, "g"), ("Huevo", 0.5m, "ud"), ("Nuez moscada", 1, "g"), ("Aceite", 5, "ml")],
            "Hervir la papa con cáscara hasta que esté tierna, pelarla y hacer un puré bien liso.",
            "Mezclar el puré tibio con el huevo y la nuez moscada.",
            "Agregar de a poco la harina de arroz y el almidón hasta lograr una masa que no se pegue.",
            "Formar cilindros, cortar los ñoquis y marcarlos con un tenedor.",
            "Cocinar en agua hirviendo hasta que suban a la superficie y servir con un hilo de aceite o la salsa elegida.");

        Rec("Tarta de pollo y garbanzos", "🥧", "Plato principal", "Argentina", 50, Dificultad.Media, 640,
            "Tarta con masa de harina de garbanzos, rellena de pollo y verduras.",
            [("Harina de garbanzos", 80, "g"), ("Pechuga de pollo", 120, "g"), ("Cebolla", 50, "g"), ("Pimiento", 40, "g"), ("Huevo", 2, "ud"), ("Aceite", 10, "ml")],
            "Mezclar la harina de garbanzos con la mitad del aceite y agua hasta formar una masa; estirarla en un molde chico.",
            "Picar la cebolla y el pimiento y rehogarlos con el resto del aceite.",
            "Cortar el pollo en cubos, sumarlo a la sartén y cocinar hasta que no quede rosado.",
            "Batir las claras, mezclarlas con el relleno y volcar sobre la masa.",
            "Hornear a 180 °C unos 25 minutos, hasta que la masa esté dorada.");

        Rec("Arepas tradicionales de maíz", "🫓", "Panificados y snacks", "Latinoamericana", 30, Dificultad.Media, 420,
            "Arepas de harina de maíz precocida rellenas de queso fresco.",
            [("Harina de maíz precocida", 80, "g"), ("Agua", 100, "ml"), ("Queso fresco", 30, "g"), ("Aceite", 5, "ml")],
            "Mezclar la harina con el agua tibia y amasar hasta que no se pegue en las manos. Dejar reposar 5 minutos.",
            "Formar bollos y aplastarlos en discos de 1 cm de espesor.",
            "Cocinar en una plancha con el aceite, 6 a 8 minutos por lado.",
            "Abrir al medio y rellenar con el queso fresco.");

        Rec("Galletas de avena certificada", "🍪", "Panificados y snacks", "Saludable, Rápido", 25, Dificultad.Facil, 450,
            "Galletas de avena y banana, sin azúcar agregada y sin TACC.",
            [("Avena sin gluten", 70, "g"), ("Banana", 100, "g"), ("Almendras", 15, "g"), ("Esencia de vainilla", 3, "ml")],
            "Pisar la banana hasta hacer un puré.",
            "Agregar la avena, las almendras picadas y la vainilla, y mezclar.",
            "Formar galletas sobre una placa con papel manteca.",
            "Hornear a 180 °C durante 15 minutos.");

        Rec("Pastel de carne y papas", "🥘", "Plato principal", "Argentina, Comfort Food", 50, Dificultad.Media, 440,
            "El clásico pastel de papas casero.",
            [("Carne vacuna", 150, "g"), ("Papa", 150, "g"), ("Cebolla", 50, "g"), ("Huevo", 0.5m, "ud"), ("Aceitunas", 15, "g")],
            "Hervir la papa y hacer un puré.",
            "Rehogar la cebolla picada y agregar la carne picada hasta que se dore.",
            "Sumar el huevo duro picado y las aceitunas al relleno.",
            "En una fuente, poner el relleno y cubrir con el puré.",
            "Gratinar en el horno a 200 °C unos 15 minutos.");

        Rec("Risotto de hongos", "🍚", "Plato principal", "Italiana, Comfort Food", 35, Dificultad.Media, 470,
            "Risotto cremoso de champiñones con parmesano.",
            [("Arroz", 80, "g"), ("Champiñones", 70, "g"), ("Cebolla", 30, "g"), ("Caldo de verduras", 250, "ml"), ("Queso parmesano", 15, "g"), ("Aceite de oliva", 10, "ml")],
            "Rehogar la cebolla picada en el aceite de oliva.",
            "Agregar los champiñones fileteados y cocinar 3 minutos.",
            "Sumar el arroz y tostarlo 1 minuto revolviendo.",
            "Agregar el caldo caliente de a un cucharón, revolviendo, hasta que el arroz esté cremoso (unos 18 minutos).",
            "Apagar el fuego, incorporar el parmesano y servir.");

        Rec("Pizza con masa de mandioca", "🍕", "Plato principal", "Italiana, Comfort Food", 40, Dificultad.Dificil, 590,
            "Pizza sin TACC con masa de fécula de mandioca y papa.",
            [("Fécula de mandioca", 90, "g"), ("Papa", 40, "g"), ("Huevo", 1, "ud"), ("Salsa de tomate", 50, "g"), ("Queso muzzarella", 50, "g")],
            "Mezclar la fécula con el puré de papa y el huevo hasta lograr una masa lisa.",
            "Estirar la masa en una pizzera aceitada.",
            "Precocinar la masa 10 minutos a 220 °C.",
            "Cubrir con la salsa de tomate y la muzzarella.",
            "Hornear 10 minutos más, hasta que el queso se derrita.");

        Rec("Wok de arroz integral y carne", "🥢", "Plato principal", "Asiática", 25, Dificultad.Facil, 375,
            "Salteado rápido de carne, verduras y arroz integral.",
            [("Arroz integral", 120, "g"), ("Carne vacuna", 120, "g"), ("Zanahoria", 50, "g"), ("Zucchini", 50, "g"), ("Salsa de soja", 15, "ml")],
            "Cortar la carne y las verduras en tiras finas.",
            "Saltear la carne en un wok bien caliente hasta dorarla.",
            "Agregar la zanahoria y el zucchini y saltear 3 minutos.",
            "Sumar el arroz integral cocido y la salsa de soja, mezclar y servir.");

        Rec("Tortilla de papas tradicional", "🍳", "Plato principal", "Mediterránea, Argentina", 35, Dificultad.Media, 435,
            "Tortilla de papas con cebolla, jugosa por dentro.",
            [("Papa", 180, "g"), ("Huevo", 2, "ud"), ("Cebolla", 60, "g"), ("Aceite de oliva", 15, "ml")],
            "Cortar la papa en láminas finas y la cebolla en juliana.",
            "Cocinarlas a fuego bajo en el aceite hasta que estén tiernas.",
            "Batir los huevos y mezclarlos con la papa y la cebolla escurridas.",
            "Cocinar en una sartén chica a fuego medio; dar vuelta con un plato y terminar del otro lado.");

        Rec("Sopa de fideos de arroz", "🍜", "Sopa o guiso", "Asiática, Comfort Food", 20, Dificultad.Facil, 340,
            "Sopa reconfortante de fideos de arroz con pollo.",
            [("Fideos de arroz", 60, "g"), ("Caldo de verduras", 300, "ml"), ("Zanahoria", 40, "g"), ("Pechuga de pollo", 60, "g")],
            "Calentar el caldo y agregar la zanahoria en rodajas.",
            "Sumar el pollo desmenuzado y cocinar 5 minutos.",
            "Agregar los fideos de arroz y cocinar 3 a 4 minutos, hasta que estén tiernos.");

        Rec("Merluza al papillote vegetal", "🐟", "Plato principal", "Saludable, Mediterránea", 30, Dificultad.Facil, 265,
            "Merluza al horno en papel, con verduras y limón. Sin sal.",
            [("Merluza", 150, "g"), ("Puerro", 40, "g"), ("Zanahoria", 40, "g"), ("Pimiento", 40, "g"), ("Limón", 20, "ml"), ("Aceite de oliva", 10, "ml")],
            "Cortar el puerro, la zanahoria y el pimiento en tiras finas.",
            "Poner las verduras sobre papel aluminio o manteca y apoyar encima el filet.",
            "Rociar con el jugo de limón y el aceite de oliva, y cerrar el paquete.",
            "Hornear a 200 °C durante 18 minutos.");

        Rec("Hamburguesa de lentejas", "🍔", "Plato principal", "Saludable, Vegana", 30, Dificultad.Media, 300,
            "Hamburguesas caseras de lentejas y avena, sin sal.",
            [("Lentejas", 150, "g"), ("Cebolla de verdeo", 40, "g"), ("Comino", 1, "g"), ("Perejil", 5, "g"), ("Avena", 30, "g")],
            "Pisar las lentejas cocidas con un tenedor.",
            "Agregar el verdeo y el perejil picados, el comino y la avena; mezclar.",
            "Formar hamburguesas y dejarlas 10 minutos en la heladera.",
            "Cocinar en una plancha caliente, 4 minutos por lado.");

        Rec("Ensalada de quínoa y vegetales", "🥗", "Ensalada", "Saludable, Latinoamericana, Rápido", 15, Dificultad.Facil, 315,
            "Ensalada fresca de quínoa, palta y verduras.",
            [("Quínoa", 120, "g"), ("Tomate", 60, "g"), ("Pepino", 50, "g"), ("Palta", 40, "g"), ("Limón", 15, "ml"), ("Aceite de oliva", 10, "ml")],
            "Enjuagar y cocinar la quínoa; dejarla enfriar.",
            "Cortar el tomate, el pepino y la palta en cubos.",
            "Mezclar todo y aliñar con el jugo de lima o limón y el aceite de oliva.");

        Rec("Pollo al curry express", "🍛", "Plato principal", "India, Rápido", 20, Dificultad.Facil, 305,
            "Pollo al curry con leche de coco y manzana.",
            [("Pechuga de pollo", 150, "g"), ("Curry en polvo", 3, "g"), ("Leche de coco", 50, "ml"), ("Manzana", 40, "g")],
            "Cortar el pollo en cubos y dorarlo en una sartén.",
            "Agregar el curry y revolver 1 minuto.",
            "Sumar la leche de coco y la manzana picada; cocinar 8 minutos a fuego bajo.");

        Rec("Guiso de porotos al natural", "🍲", "Sopa o guiso", "Argentina, Comfort Food", 30, Dificultad.Facil, 190,
            "Guiso de porotos con calabaza, sin conservas.",
            [("Porotos", 120, "g"), ("Salsa de tomate", 80, "g"), ("Calabaza", 60, "g"), ("Pimentón", 2, "g")],
            "Cortar la calabaza en cubos chicos.",
            "Cocinar la calabaza con el tomate triturado y el pimentón 10 minutos.",
            "Agregar los porotos cocidos y cocinar 10 minutos más a fuego bajo.");

        Rec("Ensalada de garbanzos y menta", "🥗", "Ensalada", "Medio Oriente, Rápido", 10, Dificultad.Facil, 300,
            "Ensalada fresca de garbanzos con pepino y menta.",
            [("Garbanzos", 120, "g"), ("Menta", 5, "g"), ("Pepino", 60, "g"), ("Limón", 20, "ml"), ("Aceite de oliva", 10, "ml")],
            "Enjuagar los garbanzos cocidos.",
            "Cortar el pepino en cubos y picar la menta.",
            "Mezclar todo y aliñar con el jugo de limón y el aceite de oliva.");

        Rec("Puré duquesa sin sodio", "🥔", "Acompañamiento", "Comfort Food", 35, Dificultad.Media, 200,
            "Puré de papas gratinado, sin sal agregada.",
            [("Papa", 180, "g"), ("Huevo", 1, "ud"), ("Nuez moscada", 1, "g"), ("Leche", 20, "ml")],
            "Hervir la papa y hacer un puré con la leche.",
            "Agregar la yema y la nuez moscada y mezclar bien.",
            "Con una manga, formar copitos sobre una placa.",
            "Gratinar en el horno a 200 °C unos 12 minutos.");

        Rec("Fajitas integrales con palta", "🌯", "Plato principal", "Mexicana", 20, Dificultad.Facil, 380,
            "Fajitas de pollo y verduras en tortilla integral.",
            [("Tortilla de trigo", 1, "ud"), ("Pechuga de pollo", 100, "g"), ("Cebolla", 40, "g"), ("Pimiento", 40, "g"), ("Palta", 40, "g")],
            "Cortar el pollo, la cebolla y el pimiento en tiras.",
            "Saltear todo en una sartén caliente hasta que el pollo esté cocido.",
            "Calentar la tortilla, untar la palta pisada y rellenar con el salteado.");

        Rec("Salmón con espárragos asados", "🐟", "Plato principal", "Saludable", 25, Dificultad.Facil, 420,
            "Salmón al horno con espárragos y limón.",
            [("Salmón", 150, "g"), ("Espárragos", 100, "g"), ("Aceite de oliva", 10, "ml"), ("Limón", 15, "ml")],
            "Poner el salmón y los espárragos en una placa.",
            "Rociar con el aceite de oliva y el jugo de limón.",
            "Hornear a 200 °C durante 15 minutos.");

        Rec("Ensalada Waldorf diabéticos", "🍏", "Ensalada", "Saludable, Rápido", 10, Dificultad.Facil, 160,
            "Versión sin azúcar de la Waldorf: manzana, apio, nueces y yogur.",
            [("Manzana", 60, "g"), ("Apio", 40, "g"), ("Nueces", 15, "g"), ("Yogur natural", 40, "g")],
            "Cortar la manzana con cáscara y el apio en cubos.",
            "Agregar las nueces en trozos.",
            "Mezclar con el yogur natural sin azúcar y servir fría.");

        Rec("Yogur con frutos rojos y lino", "🍓", "Desayuno y merienda", "Saludable, Rápido", 5, Dificultad.Facil, 140,
            "Desayuno rápido de yogur con frutos rojos y semillas.",
            [("Yogur natural", 150, "g"), ("Frutos rojos", 50, "g"), ("Semillas de lino", 10, "g")],
            "Servir el yogur en un bowl.",
            "Agregar los frutos rojos lavados y espolvorear las semillas de lino molidas.");

        Rec("Curry de garbanzos y coco", "🍛", "Plato principal", "India, Vegana", 25, Dificultad.Facil, 400,
            "Curry vegano de garbanzos y espinaca con arroz basmati.",
            [("Garbanzos", 120, "g"), ("Leche de coco", 60, "ml"), ("Curry en polvo", 3, "g"), ("Espinaca", 50, "g"), ("Arroz", 40, "g")],
            "Cocinar el arroz basmati.",
            "En una olla, calentar la leche de coco con el curry.",
            "Agregar los garbanzos y cocinar 8 minutos.",
            "Sumar la espinaca al final, hasta que se ablande, y servir con el arroz.");

        Rec("Falafels horneados de cilantro", "🧆", "Plato principal", "Medio Oriente, Vegana", 40, Dificultad.Media, 460,
            "Falafels al horno de garbanzos, perejil y cilantro.",
            [("Garbanzos", 100, "g"), ("Ajo", 1, "ud"), ("Perejil", 10, "g"), ("Cilantro", 5, "g"), ("Comino", 1, "g"), ("Aceite de oliva", 10, "ml")],
            "Procesar los garbanzos remojados (crudos) con el ajo, el perejil, el cilantro y el comino.",
            "Formar bolitas y aplastarlas apenas.",
            "Pincelar con el aceite de oliva y hornear a 200 °C unos 25 minutos, dando vuelta a la mitad.");

        Rec("Hummus tradicional con bastones", "🫘", "Acompañamiento", "Medio Oriente, Vegana, Rápido", 10, Dificultad.Facil, 330,
            "Hummus casero con bastones de zanahoria y pepino.",
            [("Garbanzos", 120, "g"), ("Tahini", 15, "g"), ("Limón", 15, "ml"), ("Zanahoria", 50, "g"), ("Pepino", 50, "g")],
            "Procesar los garbanzos cocidos con el tahini y el jugo de limón hasta que quede cremoso.",
            "Cortar la zanahoria y el pepino en bastones.",
            "Servir el hummus con los bastones para untar.");

        Rec("Scramble o revuelto de tofu", "🍳", "Desayuno y merienda", "Vegana, Rápido", 10, Dificultad.Facil, 160,
            "Revuelto vegano de tofu con cúrcuma, como un huevo revuelto.",
            [("Tofu", 130, "g"), ("Cúrcuma", 1, "g"), ("Levadura nutricional", 5, "g"), ("Tomate", 50, "g"), ("Aceite de oliva", 5, "ml")],
            "Desmenuzar el tofu con un tenedor.",
            "Cocinarlo en una sartén con el aceite, la cúrcuma y la levadura nutricional, 4 minutos.",
            "Agregar los tomates cherry cortados al medio y servir.");

        Rec("Burrito vegano de porotos", "🌯", "Plato principal", "Mexicana, Vegana", 15, Dificultad.Facil, 390,
            "Burrito de porotos negros, choclo y palta.",
            [("Tortilla de trigo", 1, "ud"), ("Porotos", 90, "g"), ("Choclo", 20, "g"), ("Lechuga", 30, "g"), ("Palta", 30, "g")],
            "Calentar los porotos sazonados con el choclo.",
            "Calentar la tortilla en una sartén.",
            "Rellenar con los porotos, la lechuga picada y la palta, y enrollar.");

        Rec("Porridge de avena y nueces", "🥣", "Desayuno y merienda", "Saludable, Vegana, Rápido", 10, Dificultad.Facil, 290,
            "Avena cocida con canela y nueces.",
            [("Avena", 50, "g"), ("Agua", 150, "ml"), ("Nueces", 15, "g"), ("Canela", 1, "g")],
            "Cocinar la avena con el agua a fuego bajo, revolviendo, 5 minutos.",
            "Servir y agregar las nueces picadas y la canela.");

        Rec("Crema de palta y cacao", "🍫", "Postre", "Vegana, Saludable", 10, Dificultad.Facil, 235,
            "Mousse de chocolate vegana hecha con palta.",
            [("Palta", 100, "g"), ("Cacao amargo", 15, "g"), ("Jarabe de agave", 10, "ml"), ("Esencia de vainilla", 2, "ml")],
            "Procesar la palta madura con el cacao, el agave y la vainilla hasta que quede liso.",
            "Llevar a la heladera 30 minutos antes de servir.");

        Rec("Arroz con leche de almendras", "🍚", "Postre", "Comfort Food, Latinoamericana", 35, Dificultad.Facil, 215,
            "Arroz con leche sin lactosa, con canela y limón.",
            [("Arroz", 50, "g"), ("Leche de almendras", 180, "ml"), ("Canela", 1, "ud"), ("Limón", null, null), ("Edulcorante", 5, "ml")],
            "Cocinar el arroz con la leche de almendras, la canela en rama y la ralladura de limón a fuego bajo.",
            "Revolver seguido durante 25 minutos, hasta que espese.",
            "Retirar la canela, agregar el edulcorante y servir tibio o frío.");

        Rec("Compota de peras natural", "🍐", "Postre", "Saludable", 25, Dificultad.Facil, 85,
            "Peras cocidas con naranja y clavo de olor.",
            [("Pera", 150, "g"), ("Agua", 100, "ml"), ("Naranja", null, null), ("Clavo de olor", 1, "ud")],
            "Pelar la pera y cortarla en cuartos.",
            "Cocinarla en el agua con la cáscara de naranja y el clavo de olor, 15 minutos a fuego bajo.",
            "Dejar enfriar en su almíbar.");

        Rec("Milanesa de berenjena al horno", "🍆", "Plato principal", "Argentina, Vegana", 30, Dificultad.Facil, 160,
            "Milanesas veganas de berenjena, rebozadas con mostaza en vez de huevo.",
            [("Berenjena", 150, "g"), ("Pan rallado", 30, "g"), ("Mostaza", 15, "ml")],
            "Cortar la berenjena en rodajas de 1 cm.",
            "Pasar cada rodaja por la mostaza disuelta en un poco de agua y después por el pan rallado.",
            "Hornear a 200 °C unos 20 minutos, dando vuelta a la mitad.");

        Rec("Guiso de lentejas turcas", "🍲", "Sopa o guiso", "Medio Oriente, Vegana", 30, Dificultad.Facil, 310,
            "Guiso rápido de lentejas rojas y calabaza.",
            [("Lentejas", 80, "g"), ("Calabaza", 80, "g"), ("Caldo de verduras", 200, "ml"), ("Pimentón", 2, "g")],
            "Cortar la calabaza en dados.",
            "Cocinar las lentejas rojas con la calabaza, el caldo y el pimentón, unos 20 minutos.",
            "Servir cuando las lentejas estén deshechas y el guiso espeso.");

        db.Restricciones.AddRange(restricciones.Values);
        db.Alergenos.AddRange(alergenos.Values);
        db.Gustos.AddRange(gustos.Values);
        db.Categorias.AddRange(categorias.Values);
        db.Ingredientes.AddRange(ingredientes.Values);
        db.Recetas.AddRange(recetas);
        db.SaveChanges();
    }

    // Admin y usuario de demostración. Las contraseñas salen de la configuración (DatosIniciales:ClaveAdmin
    // y DatosIniciales:ClaveDemo): en local, de appsettings.Development.json; en producción, de variables de entorno.
    private static void CargarUsuarios(AppDbContext db, IConfiguration config)
    {
        var hasher = new PasswordHasher<Usuario>();

        var claveAdmin = config["DatosIniciales:ClaveAdmin"];
        if (!string.IsNullOrEmpty(claveAdmin))
        {
            var admin = new Usuario { Email = "admin@cenaapp.com", Nombre = "Administrador", Rol = Rol.Admin };
            admin.PasswordHash = hasher.HashPassword(admin, claveAdmin);
            db.Usuarios.Add(admin);
        }

        var claveDemo = config["DatosIniciales:ClaveDemo"];
        if (!string.IsNullOrEmpty(claveDemo))
        {
            // Perfil armado para que se note el filtrado: vegetariana, alérgica a frutos secos,
            // y con una despensa que alcanza para la tortilla de papas.
            var demo = new Usuario
            {
                Email = "demo@cenaapp.com",
                Nombre = "Sofi",
                Nivel = Nivel.Intermedio,
                Restricciones = db.Restricciones.Where(r => r.Nombre == "Vegetariana").ToList(),
                Alergenos = db.Alergenos.Where(a => a.Nombre == "Frutos secos").ToList(),
                Gustos = db.Gustos.Where(g => g.Nombre == "Italiana" || g.Nombre == "Saludable" || g.Nombre == "Rápido").ToList(),
                Despensa = db.Ingredientes
                    .Where(i => new[] { "Papa", "Huevo", "Cebolla", "Tomate", "Arroz", "Zanahoria", "Limón", "Garbanzos" }.Contains(i.Nombre))
                    .ToList(),
            };
            demo.PasswordHash = hasher.HashPassword(demo, claveDemo);
            db.Usuarios.Add(demo);
        }

        db.SaveChanges();
    }

    private static IEnumerable<string> Lista(string nombres) =>
        nombres.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
