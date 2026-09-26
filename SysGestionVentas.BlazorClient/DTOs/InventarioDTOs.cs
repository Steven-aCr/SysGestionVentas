namespace SysGestionVentas.BlazorClient.DTOs
{
    public class InventarioDTOs
    {
        public long Id { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = "Almacén Principal";
        public int CantidadStock { get; set; }
        public int StockMinimo { get; set; }
    }
}