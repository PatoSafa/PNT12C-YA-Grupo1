// Datos de las pantallas de perfil: el formulario (onboarding y edición) y la vista de solo lectura.
using System.ComponentModel.DataAnnotations;
using CenaApp.Web.Models;

namespace CenaApp.Web.ViewModels;

// Formulario de perfil. Se usa igual para el onboarding (cuenta nueva) y para editar.
public class PerfilFormularioViewModel
{
    // VAL-06 / VAL-07
    [Required(ErrorMessage = "Contanos cómo te llamás.")]
    [StringLength(50, ErrorMessage = "El nombre puede tener hasta 50 caracteres.")]
    [Display(Name = "¿Cómo te llamás?")]
    public string Nombre { get; set; } = "";

    // VAL-08: un nivel, Intermedio por defecto.
    public Nivel Nivel { get; set; } = Nivel.Intermedio;

    // Lo que el usuario marcó (VAL-25: todo opcional).
    public List<int> RestriccionesIds { get; set; } = [];
    public List<int> AlergenosIds { get; set; } = [];
    public List<int> GustosIds { get; set; } = [];

    // true si es la primera vez (cambia títulos y oculta la barra inferior).
    public bool EsOnboarding { get; set; }

    // Opciones para dibujar el formulario (no vienen del POST: las vuelve a cargar el controller).
    public List<Restriccion> Restricciones { get; set; } = [];
    public List<Alergeno> Alergenos { get; set; } = [];
    public List<Gusto> Gustos { get; set; } = [];
}

// Perfil en modo lectura.
public class PerfilViewModel
{
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public Nivel Nivel { get; set; }
    public List<string> Restricciones { get; set; } = [];
    public List<string> Alergenos { get; set; } = [];
    public List<string> Gustos { get; set; } = [];
}
