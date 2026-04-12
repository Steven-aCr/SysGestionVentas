using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel para que un usuario autenticado edite su propio perfil.
    /// El cambio de contraseña es opcional: si <c>NewPassword</c> se deja vacío,
    /// la contraseña actual se conserva sin modificaciones.
    /// </summary>
    public class EditProfileModel
    {
        [Required]
        public int UserId { get; set; }

        // ── Datos personales ──────────────────────────────────────────

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string Adress { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Formato de número de teléfono inválido.")]
        [StringLength(20)]
        [Display(Name = "Número de teléfono")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Formato: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "DUI")]
        public string? Dui { get; set; }

        // ── Datos de acceso ───────────────────────────────────────────

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Introduzca un correo electrónico válido.")]
        [StringLength(255)]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        // ── Cambio de contraseña (opcional) ──────────────────────────

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string? CurrentPassword { get; set; }

        [StringLength(255, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre 8 y 255 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar nueva contraseña")]
        public string? ConfirmNewPassword { get; set; }

        // Solo lectura
        public int PersonId { get; set; }
    }
}