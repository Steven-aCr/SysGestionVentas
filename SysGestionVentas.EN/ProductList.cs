using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class ProductList
    {
        [Key]
        public int ProductoId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio")]
        [StringLength(150)]
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio")]
        [StringLength(50)]
        public string? Barcode { get; set; }

        public DateTime CreatedAt { get; set; }

        public int StatusId { get; set; }

        public int CategoryId { get; set; }

        public int CreatedByUser { get; set; }
    }
}
