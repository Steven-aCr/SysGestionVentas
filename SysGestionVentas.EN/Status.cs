using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength =6,
            ErrorMessage ="El nombre debe tener un mínimo de 6 caracteres.")]
        [Display(Name="Nombre de estado")]
        public string? Name { get; set; }

        [StringLength(255)]
        [Display(Name ="Descripción")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "El tipo de estado es obligatorio.")]
        [ForeignKey("StatusType")]
        public int StatusTypeId { get; set; }
        public StatusType? StatusType { get; set; }

        public List<StatusType> StatusTypes { get; set; } = null!;

    }
}
