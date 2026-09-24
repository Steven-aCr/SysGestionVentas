using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Producto
{
    public class ProductoModificar
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int Id { get; set; }

        [Display(Name = "Nombre de Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string? Descripcion { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int IdCategoria { get; set; }
    }
}