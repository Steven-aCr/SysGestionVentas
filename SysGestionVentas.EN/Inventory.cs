using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "El precio de compra es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de compra debe ser mayor a $0.00.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio de compra")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor a $0.00.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio de venta")]
        public decimal SalePrice { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0.")]
        [Display(Name = "Stock mínimo")]
        public int MinimumStock { get; set; }

        [Required(ErrorMessage = "El stock actual es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock actual debe ser mayor o igual a 0.")]
        [Display(Name = "Stock actual")]
        public int CurrentStock { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public ProductList? ProductList { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
