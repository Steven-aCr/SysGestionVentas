using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    /// <summary>
    /// Entidad de unión que representa la relación muchos a muchos
    /// entre <see cref="Product"/> y <see cref="Discount"/>.
    /// Define qué descuento tiene asignado cada producto del sistema.
    /// </summary>
    public class ProductDiscount
    {
        [Required]
        public int ProductId { get; set; }
        public ProductList? Product { get; set; }

        [Required]
        public int DiscountId { get; set; }
        public Discount? Discount { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("AssignedBy")]
        public int AssignedByUser { get; set; }

        public User? AssignedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}