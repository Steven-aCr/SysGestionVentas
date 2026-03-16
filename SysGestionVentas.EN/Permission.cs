using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage ="El nombre debe tener un máximo de 100 caracteres.")]
        [Display(Name="Nombre del Permiso")]
        public string? Name { get; set; }

        [Display(Name="Descripción")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
