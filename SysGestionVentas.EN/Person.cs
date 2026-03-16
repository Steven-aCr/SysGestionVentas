using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [Required (ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name ="Primer nombre")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage ="El apellido es obligatorio.")]
        [StringLength(50, MinimumLength =2)]
        [Display(Name ="Primer apellido")]
        public string? LastName { get; set; }

        [Required(ErrorMessage ="La dirección es obligatoria")]
        [StringLength(255)]
        [Display(Name ="Dirección de residencia")]
        public string? Adress { get; set; }

        [Required(ErrorMessage ="El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage ="Formato de número de teléfono invalido.")]
        [StringLength(20)]
        [Display(Name ="Número de teléfono")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\d{8}-\d$", ErrorMessage ="Formato: 12345678-9")]
        [StringLength(10)]
        [Display(Name ="DUI")]
        public string? Dui { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        [NotMapped]
        public string FullNmae => $"{FirstName} {LastName}";
    }
}
