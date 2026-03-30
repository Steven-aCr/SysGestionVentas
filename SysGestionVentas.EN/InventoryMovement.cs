using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class InventoryMovement
    {
        [Key]
        public int InventoryMovementId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        [ForeignKey("MovementType")]
        public int MovementTypeId { get; set; }
        public MovementType? MovementType { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El costo unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo unitario debe ser mayor a $0.01.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [ForeignKey("CreatedBy")]
        public int CreatedByUser { get; set; }
        public User? CreatedBy { get; set; }

        [Required(ErrorMessage = "El inventario es obligatorio.")]
        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public DateTime MovementDate { get; set; }
    }
}