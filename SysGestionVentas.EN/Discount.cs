using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "El porcentaje es obligatorio.")]
        [Range(0.01, 100, ErrorMessage = "El porcentaje debe estar entre 0.01% y 100%.")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}