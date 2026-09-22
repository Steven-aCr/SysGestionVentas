using System;
using System.Collections.Generic;
using System.Text;
namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioSalida
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
