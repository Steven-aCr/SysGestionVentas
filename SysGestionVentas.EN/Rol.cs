using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, MinimumLength = 6,
            ErrorMessage = "El nombre debe tener entre 6 y 30 caracteres.")]
        [Display(Name = "Nombre de Rol")]
        public string? Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}