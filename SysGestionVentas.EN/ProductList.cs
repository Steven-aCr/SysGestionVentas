using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class ProductList
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, MinimumLength = 3)]
        [Display(Name = "Nombre de producto")]
        public string? Name { get; set; }

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio.")]
        [StringLength(100)]
        [RegularExpression(@"^[\w-]{3,100}$",
            ErrorMessage = "Formato de código de barras inválido.")]
        [Display(Name = "Código de barra")]
        public string? Barcode { get; set; }

        [StringLength(500)]
        [Url(ErrorMessage = "URL de imagen inválida.")]
        [Display(Name = "URL de imagen")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
<<<<<<< HEAD
        public Category? category { get; set; }
        public object Category { get; set; }
=======
        public Category? Category { get; set; }

>>>>>>> Dev2
        [Required]
        [ForeignKey("CreatedBy")]
        public int CreatedByUser { get; set; }
        public User? CreatedBy { get; set; }

        public Inventory? Inventory { get; set; }
        public object Product { get; set; }
        public int ProductId { get; set; }
    }
}