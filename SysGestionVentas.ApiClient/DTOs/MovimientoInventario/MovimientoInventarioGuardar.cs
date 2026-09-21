namespace SysGestionVentas.ApiClient.DTOs.MovimientoInventario
{
    public class MovimientoInventarioGuardar
    {
        public int MovementTypeId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public string? Notes { get; set; }

        public int CreatedByUser { get; set; }

        public int InventoryId { get; set; }

        public int? DocumentDetailId { get; set; }
    }
}