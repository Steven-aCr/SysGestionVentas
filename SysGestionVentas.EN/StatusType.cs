using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class StatusType
    {
        [Key]
        public int StatusTypeId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "El nombre debe tener entre 6 y 100 caracteres.")]
        [Display(Name = "Tipo de estado")]
        public string? Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Status>? Status { get; set; }
    }
}