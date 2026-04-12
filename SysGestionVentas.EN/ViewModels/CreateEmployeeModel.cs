using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel utilizado para capturar en un único formulario los datos
    /// necesarios para crear una <see cref="Person"/> y su <see cref="Employee"/> asociado.
    /// Permite que ambos registros se persistan en una sola transacción coordinada.
    /// </summary>
    public class CreateEmployeeModel
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

        // ── Datos de Empleado ────────────────────────────────────────

        [Required(ErrorMessage = "El código de empleado es obligatorio.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El código de empleado debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Código del empleado")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de contratación es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de contratación")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El salario debe ser mayor a $0.00.")]
        [Display(Name = "Salario")]
        public decimal Salary { get; set; }

        [Display(Name = "Departamento")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Usuario asociado (opcional)")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "El estado del empleado es obligatorio.")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }
    }
}