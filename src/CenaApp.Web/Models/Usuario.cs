// Persona registrada: datos de la cuenta, su perfil y sus listas (favoritos y despensa).
using System.ComponentModel.DataAnnotations;

namespace CenaApp.Web.Models;

public class Usuario
{
    public int Id { get; set; }

    // Se guarda siempre en minúsculas para que el email sea único sin importar mayúsculas (VN-01).
    [Required, MaxLength(254)]
    public string Email { get; set; } = "";

    // Nunca la contraseña: solo el hash que genera PasswordHasher (RNF13).
    [Required]
    public string PasswordHash { get; set; } = "";

    public Rol Rol { get; set; } = Rol.Usuario;

    [Required, MaxLength(50)]
    public string Nombre { get; set; } = "";

    public Nivel Nivel { get; set; } = Nivel.Intermedio;

    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    // Perfil
    public List<Restriccion> Restricciones { get; set; } = [];
    public List<Alergeno> Alergenos { get; set; } = [];
    public List<Gusto> Gustos { get; set; } = [];

    // Listas del usuario
    public List<Ingrediente> Despensa { get; set; } = [];
    public List<Receta> Favoritos { get; set; } = [];
}
