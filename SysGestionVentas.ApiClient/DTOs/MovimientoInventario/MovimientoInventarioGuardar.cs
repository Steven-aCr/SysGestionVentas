using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.MovimientoInventario
{
    public class MovimientoInventarioGuardar
    {
        [Required]
        [Range(1, 5)]
        public int IdTipoMovimiento { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal CostoUnitario { get; set; }

        public string? Notas { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdInventario { get; set; }

        public int? IdDetalleDocumento { get; set; }
    }
}