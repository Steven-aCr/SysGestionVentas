using System;
using System.Collections.Generic;
using System.Text;

namespace SysGestionVentas.ApiClient.DTOs.Producto
{
    public class ProductoSalida
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
    }
}