// Despensa con AJAX (Parte 15 del enunciado): agregar, quitar y vaciar sin recargar la página.
// Recorrido: formulario → fetch POST → DespensaController → DespensaService → PostgreSQL
//            → el servidor responde el HTML de la vista parcial _Contenido → se reemplaza en la página.

const contenido = document.getElementById("contenido-despensa");
const formAgregar = document.getElementById("form-agregar");

// Manda un formulario con fetch y pone en la página el HTML que responde el servidor.
async function enviar(formulario) {
    const datos = new FormData(formulario);
    // El token antiforgery está dentro del bloque reemplazable (_Contenido); se toma el más reciente.
    datos.set("__RequestVerificationToken", contenido.querySelector('input[name="__RequestVerificationToken"]').value);

    try {
        const respuesta = await fetch(formulario.action, {
            method: "POST",
            body: datos,
            headers: { "X-Requested-With": "fetch" }, // así el servidor sabe que tiene que responder solo el bloque
        });
        if (!respuesta.ok) {
            throw new Error(`El servidor respondió ${respuesta.status}`);
        }
        contenido.innerHTML = await respuesta.text();
        return true;
    } catch (error) {
        console.error(error);
        alert("No se pudo actualizar la despensa. Probá de nuevo.");
        return false;
    }
}

// Formulario principal (input + botón).
formAgregar.addEventListener("submit", async (evento) => {
    evento.preventDefault();
    if (await enviar(formAgregar)) {
        formAgregar.reset();
        formAgregar.querySelector("input[name=texto]").focus();
    }
});

// Formularios dentro del bloque (accesos rápidos, quitar con ×, limpiar todo). Como el bloque se
// reemplaza en cada respuesta, se escucha en el contenedor en vez de en cada formulario.
contenido.addEventListener("submit", async (evento) => {
    const formulario = evento.target.closest(".form-despensa");
    if (!formulario) {
        return;
    }
    evento.preventDefault();
    if (formulario.dataset.confirmar && !confirm(formulario.dataset.confirmar)) {
        return;
    }
    await enviar(formulario);
});
