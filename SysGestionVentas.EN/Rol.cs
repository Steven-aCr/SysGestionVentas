using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(30, MinimumLength =6, 
            ErrorMessage ="El nombre debe tener entre 6 y 30 caracteres.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
