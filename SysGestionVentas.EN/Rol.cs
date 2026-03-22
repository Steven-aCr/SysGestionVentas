using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(30, MinimumLength =6, 
            ErrorMessage ="El nombre debe tener un máximo de 30 caracteres.")]
        [Display(Name ="Nombre de Rol")]
        public string? Name { get; set; }

        [Display(Name ="Descripción")]
        [StringLength(200, MinimumLength = 3, 
            ErrorMessage ="La descripción debe tener un máximo de 200 caracteres.")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
