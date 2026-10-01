using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioModificar
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdInventario { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal PrecioCompra { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal PrecioVenta { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdEstado { get; set; }
    }
}
