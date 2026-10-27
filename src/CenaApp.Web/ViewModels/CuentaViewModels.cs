// Datos de los formularios de registro e inicio de sesión, con sus validaciones (VAL-01 a VAL-05).
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.ViewModels;

public class RegistroViewModel
{
    [Required(ErrorMessage = "Ingresá tu email.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El email puede tener hasta 254 caracteres.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Ingresá una contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña tiene que tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Repetí la contraseña.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Repetir contraseña")]
    public string ConfirmarPassword { get; set; } = "";
}

public class IngresoViewModel
{
    [Required(ErrorMessage = "Ingresá tu email.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    // A dónde volver después de ingresar (la página que pidió login).
    public string? ReturnUrl { get; set; }
}
