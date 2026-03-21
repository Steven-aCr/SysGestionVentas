using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class ProductList
    {
        [Key]
        public int ProductoId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio")]
        [StringLength(150, MinimumLength =3)]
        [Display(Name ="Nombre de producto")]
        public string? Name { get; set; }

        [StringLength(255)]
        [Display(Name ="Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio")]
        [StringLength(50)]
        [RegularExpression(@"^[\w-]{3,50}$", 
            ErrorMessage = "Formato de código de barras inválido.")]
        [Display(Name ="Código de barra")]
        public string? Barcode { get; set; }

        //[StringLength(255)]
        //[Url(ErrorMessage = "URL de imagen inválida.")]
        //public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage ="El estado es obligatorio.")]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? category { get; set; }
        public object Category { get; set; }
        [Required]
        [ForeignKey("CreatedBy")]
        public int CreatedByUser { get; set; }
        public User? CreatedBy { get; set; }

        public Inventory? Inventory { get; set; }
        public object Product { get; set; }
        public int ProductId { get; set; }
    }
}
