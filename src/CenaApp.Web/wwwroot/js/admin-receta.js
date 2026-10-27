// Formulario de receta (admin): agregar y quitar filas de ingredientes sin recargar.
// Cada fila tiene un número propio (Datos.Ingredientes.Index), así el servidor las lee aunque se borren filas del medio.

const contenedor = document.getElementById("filas-ingredientes");
let siguiente = contenedor.children.length;

document.getElementById("agregar-fila").addEventListener("click", () => {
    // Copia la última fila, la vacía y le pone el número nuevo en los "name".
    const nueva = contenedor.lastElementChild.cloneNode(true);
    nueva.querySelectorAll("[name]").forEach(campo => {
        campo.name = campo.name.replace(/\[\d+\]/, `[${siguiente}]`);
    });
    nueva.querySelector('input[type="hidden"]').value = siguiente;
    nueva.querySelectorAll("select").forEach(s => s.selectedIndex = 0);
    nueva.querySelector('input[type="number"]').value = "";
    contenedor.appendChild(nueva);
    siguiente++;
});

contenedor.addEventListener("click", (evento) => {
    const boton = evento.target.closest(".quitar-fila");
    if (!boton) {
        return;
    }
    const fila = boton.closest(".fila-ingrediente-admin");
    if (contenedor.children.length > 1) {
        fila.remove();
    } else {
        // Queda siempre una fila: en vez de borrarla, se vacía.
        fila.querySelectorAll("select").forEach(s => s.selectedIndex = 0);
        fila.querySelector('input[type="number"]').value = "";
    }
});
