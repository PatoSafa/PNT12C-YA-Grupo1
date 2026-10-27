// Pide confirmación antes de enviar los formularios que tienen data-confirmar="pregunta" (ej: eliminar).
document.addEventListener("submit", (evento) => {
    const pregunta = evento.target.dataset.confirmar;
    if (pregunta && !confirm(pregunta)) {
        evento.preventDefault();
    }
});
