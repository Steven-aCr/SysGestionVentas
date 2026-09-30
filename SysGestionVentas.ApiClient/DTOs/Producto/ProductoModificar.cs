using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Producto
{
    public class ProductoModificar
    {
        [Required(ErrorMessage = "El id del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El id del producto debe ser mayor a 0.")]
        public int IdProducto { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            150,
            MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres."
        )]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(
            255,
            ErrorMessage = "La descripción no puede exceder los 255 caracteres."
        )]
        public string? Descripcion { get; set; }

        [Display(Name = "Código de barras")]
        [Required(ErrorMessage = "El código de barras es obligatorio.")]
        [StringLength(
            100,
            ErrorMessage = "El código de barras no puede exceder los 100 caracteres."
        )]
        public string CodigoBarras { get; set; } = string.Empty;

        [Display(Name = "URL de imagen")]
        [StringLength(
            500,
            ErrorMessage = "La URL de la imagen no puede exceder los 500 caracteres."
        )]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Estado")]
        [Range(1, int.MaxValue, ErrorMessage = "El estado debe ser mayor a 0.")]
        public int IdEstado { get; set; }

        [Display(Name = "Categoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
        public int IdCategoria { get; set; }
    }
}