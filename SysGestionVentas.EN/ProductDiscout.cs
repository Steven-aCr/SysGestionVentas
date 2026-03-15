using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class ProductDiscout
    {
        [Required]
        public int ProductId { get; set; }
        public ProductList? Product { get; set; }

        [Required]
        public int DiscountId { get; set; }
        public Discount? Discount { get; set; }
    }
}
