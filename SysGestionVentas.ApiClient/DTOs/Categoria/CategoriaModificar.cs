using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Categoria
{
    public class CategoriaModificar
    {
        [Required(ErrorMessage = "El id de la categoría es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El id de la categoría debe ser mayor a 0.")]
        public int IdCategoria { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            50,
            MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres."
        )]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(
            255,
            ErrorMessage = "La descripción no puede exceder los 255 caracteres."
        )]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El estado debe ser mayor a 0.")]
        public int IdEstado { get; set; }
    }
}