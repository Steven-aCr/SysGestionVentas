using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioGuardar
    {
        [Required]
        [Range(1,int.MaxValue)]
        public int IdProducto { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal PrecioCompra { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockActual { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; }
    }
}
