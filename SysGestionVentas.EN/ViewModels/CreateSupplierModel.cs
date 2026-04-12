using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel utilizado para capturar en un único formulario los datos
    /// necesarios para crear una <see cref="Person"/> y su <see cref="Supplier"/> asociado.
    /// Permite que ambos registros se persistan en una sola transacción coordinada.
    /// </summary>
    public class CreateSupplierModel
    {
        // ── Datos de Persona ──────────────────────────────────────────

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre del contacto")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Apellido del contacto")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string Adress { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Formato de número de teléfono inválido.")]
        [StringLength(20)]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Formato: 1234-5678")]
        [Display(Name = "Teléfono de contacto")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Formato: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "DUI del contacto")]
        public string? Dui { get; set; }

        [Required(ErrorMessage = "El estado de la persona es obligatorio.")]
        [Display(Name = "Estado de persona")]
        public int PersonStatusId { get; set; }

        // ── Datos de Proveedor ────────────────────────────────────────

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(150, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
        [Display(Name = "Nombre de la empresa")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El NIT es obligatorio.")]
        [RegularExpression(@"^\d{4}-\d{6}-\d{3}-\d$",
            ErrorMessage = "Formato NIT: 1234-567890-123-4")]
        [StringLength(17)]
        [Display(Name = "NIT")]
        public string Nit { get; set; } = string.Empty;

        [Required(ErrorMessage = "El NRC es obligatorio.")]
        [RegularExpression(@"^\d{1,8}-\d$",
            ErrorMessage = "Formato NRC: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "NRC")]
        public string Nrc { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El estado del proveedor es obligatorio.")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }
    }
}