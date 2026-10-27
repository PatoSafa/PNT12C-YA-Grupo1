// Mapa de Tiendas con Leaflet + OpenStreetMap (D-11): un marcador por comercio y "Ver en el mapa" desde la lista.
// Los datos los deja el servidor en <script id="datos-mapa">. Si Leaflet no carga, queda la lista (RNF18).

const datos = JSON.parse(document.getElementById("datos-mapa").textContent);
const contenedor = document.getElementById("mapa");

if (typeof L === "undefined") {
    contenedor.hidden = true;
    document.getElementById("mapa-error").hidden = false;
} else {
    const mapa = L.map(contenedor).setView([datos.centro.lat, datos.centro.lng], 15);

    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    }).addTo(mapa);

    // Punto de referencia (ORT Yatay).
    L.circleMarker([datos.centro.lat, datos.centro.lng], { radius: 7, color: "#F97316", fillOpacity: 1 })
        .addTo(mapa)
        .bindPopup(`<strong>${datos.centro.nombre}</strong>`);

    // Un marcador verde por comercio. Se guardan por id para poder encontrarlos desde la lista.
    const marcadores = {};
    for (const c of datos.comercios) {
        marcadores[c.id] = L.circleMarker([c.lat, c.lng], { radius: 10, color: "#16A34A", fillColor: "#22C55E", fillOpacity: .9 })
            .addTo(mapa)
            .bindPopup(`<strong>${c.emoji} ${c.nombre}</strong><br>${c.direccion}<br>⏰ ${c.horario}`);
    }

    // "Ver en el mapa": centra el mapa en el comercio, abre su globito y marca la tarjeta.
    document.querySelectorAll(".ver-en-mapa").forEach(boton => boton.addEventListener("click", () => {
        const id = boton.dataset.comercio;
        const marcador = marcadores[id];
        mapa.flyTo(marcador.getLatLng(), 17);
        marcador.openPopup();

        document.querySelectorAll(".tarjeta-comercio").forEach(t => t.classList.toggle("elegido", t.dataset.comercio === id));
        contenedor.scrollIntoView({ behavior: "smooth", block: "center" });
    }));
}
