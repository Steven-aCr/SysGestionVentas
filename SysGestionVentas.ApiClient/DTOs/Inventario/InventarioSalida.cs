namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioSalida
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}