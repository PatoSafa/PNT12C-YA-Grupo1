// Detalle de receta: cuando se marcan todos los pasos, muestra el cartel de felicitaciones (RF15).
const pasos = document.querySelectorAll(".paso-check");
const felicitaciones = document.getElementById("felicitaciones");

pasos.forEach(paso => paso.addEventListener("change", () => {
    const todosHechos = [...pasos].every(p => p.checked);
    felicitaciones.hidden = !todosHechos;
}));
