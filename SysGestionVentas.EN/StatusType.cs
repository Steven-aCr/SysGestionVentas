using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class StatusType
    {
        [Key]
        public int StatusTypeId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength =6, 
            ErrorMessage ="El nombre debe tener un mínimo de 6 caracteres.")]
        [Display(Name ="Tipo de estado")]
        public string? Name { get; set; }

        [StringLength(255)]
        [Display(Name ="Descripción")]
        public string? Description {  get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Status>? Statuses { get; set; }
    }
}
