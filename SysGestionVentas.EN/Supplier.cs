using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
   public class Supplier
   {
        [Key]
        public int SupplierId { get; set; }

        public int PersonId { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(150)]
        public string? CompanyName { get; set; }

        public string? Nit { get; set; }

        public string? Nrc { get; set; }

        public string? Description { get; set; }

        public int StatusId { get; set; }

    }
}
