using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SysGestionVentas.EN
{
    public  class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 8,
            ErrorMessage = "El nombre debe tener entre 8 Y 50 caracteres.")]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        [Required]
        [ForeignKey("CreatedBy")]
        public int CreatedByUser { get; set; }
        public User? CreatedBy { get; set; }
    }
}
