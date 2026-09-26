namespace SysGestionVentas.BlazorClient.DTOs
{
    public class MovimientoInventarioDTOs
    {
        public long Id { get; set; }
        public long ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string TipoMovimiento { get; set; } = "ENTRADA"; // ENTRADA, SALIDA, AJUSTE
        public int Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}