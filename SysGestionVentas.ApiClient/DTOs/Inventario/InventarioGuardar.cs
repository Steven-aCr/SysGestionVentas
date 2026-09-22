using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioGuardar
    {
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int IdProducto { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "La {0} debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}
