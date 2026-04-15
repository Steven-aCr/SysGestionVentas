using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel utilizado para capturar en un único formulario los datos
    /// necesarios para crear una <see cref="Person"/>, su <see cref="User"/> asociado
    /// y, cuando el rol corresponde a Vendedor (RolId = 2), su registro de
    /// <see cref="Employee"/> en una sola transacción coordinada.
    /// </summary>
    public class CreateUserModel
    {
        // ── Datos de Person ───────────────────────────────────────────

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
        [Display(Name = "Dirección de residencia")]
        public string Adress { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Formato de número de teléfono inválido.")]
        [StringLength(20)]
        [Display(Name = "Número de teléfono")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Formato: 1234-5678")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Formato: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "DUI")]
        public string? Dui { get; set; }

        // ── Datos de Usuario ──────────────────────────────────────────

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Introduzca un correo electrónico válido.")]
        [StringLength(255)]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre 8 y 255 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }

        // ── Datos de Empleado (solo si RolId == 2 — Vendedor) ────────

        /// <summary>
        /// Código único del empleado. Requerido únicamente cuando el rol
        /// seleccionado corresponde a Vendedor (RolId = 2).
        /// </summary>
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El código de empleado debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Código de empleado")]
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Fecha de contratación del empleado. Requerida únicamente cuando
        /// el rol seleccionado corresponde a Vendedor (RolId = 2).
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de contratación")]
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Salario mensual del empleado. Requerido únicamente cuando el rol
        /// seleccionado corresponde a Vendedor (RolId = 2).
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "El salario debe ser mayor a $0.00.")]
        [Display(Name = "Salario")]
        public decimal? Salary { get; set; }

        /// <summary>
        /// Departamento al que pertenece el empleado. Opcional incluso para Vendedor.
        /// </summary>
        [Display(Name = "Departamento")]
        public int? DepartmentId { get; set; }

        // ── Constantes auxiliares ─────────────────────────────────

        /// <summary>
        /// Identificador del rol Administrador en el sistema.
        /// Se utiliza para determinar si deben persistirse los datos de empleado.
        /// </summary>
        public const int RolAdministradorId = 1;

        /// <summary>
        /// Identificador del rol Vendedor en el sistema.
        /// Se utiliza para determinar si deben persistirse los datos de empleado.
        /// </summary>
        public const int RolVendedorId = 3;
    }
}