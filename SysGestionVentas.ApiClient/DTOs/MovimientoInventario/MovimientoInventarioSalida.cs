namespace SysGestionVentas.ApiClient.DTOs.MovimientoInventario
{
    public class MovimientoInventarioSalida
    {
        public int IdMovimientoInventario { get; set; }

        public int IdTipoMovimiento { get; set; }

        public int Cantidad { get; set; }

        public decimal CostoUnitario { get; set; }

        public string? Notas { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int CreadoPorUsuario { get; set; }

        public int IdInventario { get; set; }

        public int? IdDetalleDocumento { get; set; }
    }
}