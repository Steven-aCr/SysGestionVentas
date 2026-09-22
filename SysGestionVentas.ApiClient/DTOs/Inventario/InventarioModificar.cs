using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Inventario
{
    public class InventarioModificar
    {
        [Required(ErrorMessage = "El ID del inventario es obligatorio.")]
        public int Id { get; set; }

        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int IdProducto { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "La {0} no puede ser un número negativo.")]
        public int Cantidad { get; set; }
    }
}
