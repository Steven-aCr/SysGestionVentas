
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 6, 
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Nombre de usuario")]
        public string? UserName { get; set; }

        [Required(ErrorMessage ="El correo eléctronico es obligatorio.")]
        [EmailAddress(ErrorMessage ="Introduzca un correo eléctronico válido.")]
        [Display(Name = "Correo eléctronico")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 8,
            ErrorMessage ="La contraseña debe tener un mínimo de 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name ="Contraseña")]
        public string? PasswordHash { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [ForeignKey("Rol")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }
        public Rol? Rol { get; set; }

        [Required(ErrorMessage = "La persona es obligatoria.")]
        [ForeignKey("Person")]
        [Display(Name = "Persona")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
