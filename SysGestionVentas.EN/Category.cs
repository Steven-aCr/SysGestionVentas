using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public  class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "El Nombre Es Obligatorio.")]
        [StringLength(50, MinimumLength = 8,
            ErrorMessage = "El Nombre Debe Tener Entre 8 Y 50 Caracteres.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        [Required]
        [ForeignKey("User")]
        public int CreatedByUser { get; set; }
        public User? User { get; set; }
    }
}
