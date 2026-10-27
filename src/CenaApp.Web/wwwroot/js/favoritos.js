// Corazones de favoritos: al tocar uno, avisa al servidor con fetch y cambia el ícono sin recargar la página (D-07).
// Recorrido: botón → fetch POST /Recetas/Favorito/{id} → RecetasController → RecetaService → tabla Favoritos.

// Token antiforgery que la vista dejó en la página (@Html.AntiForgeryToken()); el servidor lo exige en los POST.
const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

document.addEventListener("click", async (evento) => {
    const boton = evento.target.closest(".boton-favorito");
    if (!boton) {
        return;
    }

    boton.disabled = true;
    try {
        const respuesta = await fetch(`/Recetas/Favorito/${boton.dataset.receta}`, {
            method: "POST",
            headers: { "RequestVerificationToken": token },
        });
        if (!respuesta.ok) {
            throw new Error(`El servidor respondió ${respuesta.status}`);
        }

        const datos = await respuesta.json();   // { esFavorita: true | false }
        boton.classList.toggle("activo", datos.esFavorita);
        boton.setAttribute("aria-pressed", datos.esFavorita);
        boton.querySelector("i").className = datos.esFavorita ? "bi bi-heart-fill" : "bi bi-heart";
    } catch (error) {
        console.error(error);
        alert("No se pudo actualizar el favorito. Probá de nuevo.");
    } finally {
        boton.disabled = false;
    }
});
