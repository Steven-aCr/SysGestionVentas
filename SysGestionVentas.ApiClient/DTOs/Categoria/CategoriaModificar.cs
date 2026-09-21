using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.ApiClient.DTOs.Categoria
{
    public class CategoriaModificar
    {
        [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
        public int Id { get; set; }

        [Display(Name = "Nombre de Categoría")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}