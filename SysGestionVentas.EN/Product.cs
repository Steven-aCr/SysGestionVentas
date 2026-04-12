using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
