

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class DocumentDetail
    {
        [Key]
        public int DocDetailId { get; set; }

        [Required]
        [ForeignKey("Document")]
        public int DocumentId { get; set; }

        public Document? Document { get; set; }

        [Required]
        [ForeignKey("ProductList")]
        public int ProductId { get; set; }
        public ProductList? ProductList { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }
}
