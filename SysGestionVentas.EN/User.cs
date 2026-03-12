
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 6, 
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage ="El correo eléctronico es obligatorio.")]
        [EmailAddress(ErrorMessage ="Introduzca un correo eléctronico valido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 8,
            ErrorMessage ="La contraseña debe tener un mínimo de 8 caracteres.")]
        public string? PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public int PersonId { get; set; }
        public Person? Person { get; set; }
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
