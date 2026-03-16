using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class DocumentDetail
    {
        [Key]
        public int DocDetailId { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        public Document? Document { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public ProductList? Product { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor o igual a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        //[Range(0, 100)]
        //[Column(TypeName = "decimal(5,2)")]
        //public decimal TaxPercentage { get; set; } = 13;

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal TaxAmount { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal TotalAmount { get; set; }

        //[StringLength(255)]
        //public string? Notes { get; set; }
    }
}