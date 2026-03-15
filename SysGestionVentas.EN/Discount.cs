using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public  class Discount
    {
        [Key]
        public int DiscountId { get; set; }
        [Required(ErrorMessage = "El Nombre Es Oblicatorio.")]

        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "El Nombre Debe Tener Entre 6 Y 100 Caracteres.")]

        public string Name { get; set; } = string.Empty;
        [Required]

        public decimal Percentage { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public bool IsActive { get; set; }
    }
}
