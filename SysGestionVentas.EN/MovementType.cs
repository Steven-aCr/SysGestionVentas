using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class MovementType
    {
        [Key]
        public int MovementTypeId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}