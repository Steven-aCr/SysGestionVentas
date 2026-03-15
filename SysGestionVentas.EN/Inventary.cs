using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class Inventary
    {
        [Key]
        public int InventoryId { get; set; }
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public int MinimumStock { get; set; }
        [Required]
        public int CurrentStock { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public ProductList? ProductList { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
