using System;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public  class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "El nombre es oblicatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre dDebe tener tntre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
       
        [Required(ErrorMessage ="El porcentaje es obligatorio.")]
        [Range(0.01, 100, ErrorMessage ="El porcentaje debe ser entre 0.01% y 100%")]
        public decimal Percentage { get; set; }

        [Required(ErrorMessage ="La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage ="La fecha de fin es obligatoria.")]
        [DataType (DataType.Date)]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
