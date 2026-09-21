namespace SysGestionVentas.ApiClient.DTOs.MovimientoInventario
{
    public class MovimientoInventarioSalida
    {
        public int IdMovimientoInventario { get; set; }

        public int IdTipoMovimiento { get; set; }

        public int Cantidad { get; set; }

        public decimal CostoUnitario { get; set; }

        public string? Notas { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int CreadoPorUsuario { get; set; }

        public int IdInventario { get; set; }

        public int? IdDetalleDocumento { get; set; }

        public int? StockAnterior { get; set; }

        public int? StockNuevo { get; set; }
    }
}