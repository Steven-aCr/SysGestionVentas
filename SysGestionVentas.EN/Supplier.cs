using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
   public class Supplier
   {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(150, MinimumLength = 3)]
        [Display(Name = "Nombre de la Empresa")]
        public string? CompanyName { get; set; }

        [RegularExpression(@"^\d{4}-\d{6}-\d{3}-\d$",
            ErrorMessage = "Formato NIT: 1234-567890-123-4")]
        [StringLength(17)]
        [Display(Name = "NIT")]
        public string? Nit { get; set; }

        [RegularExpression(@"^\d{1,8}-\d$", 
            ErrorMessage = "Formato NRC: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "NRC")]
        public string? Nrc { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(255)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
