
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class InventoryMovement
    {
        [Key]
        public int InventoryMovementId { get; set; }

        [Required]
        public bool MovementType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("User")]
        public int CreatedByuser { get; set; }
        public User? User { get; set; }

        [Required]
        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
    }
}
