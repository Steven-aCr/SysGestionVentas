using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage ="El nombre debe tener un mínimo de 6 caracteres.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
