using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 6,
            ErrorMessage = "El nombre debe tener entre 6 y 50 caracteres.")]
        [Display(Name = "Nombre de estado")]
        public string? Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "El tipo de estado es obligatorio.")]
        [ForeignKey("StatusType")]
        public int StatusTypeId { get; set; }
        public StatusType? StatusType { get; set; }
    }
}