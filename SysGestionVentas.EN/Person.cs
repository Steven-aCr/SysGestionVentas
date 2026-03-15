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
        public string? FirstName { get; set; }

        [Required(ErrorMessage ="El apellido es obligatorio.")]
        [StringLength(50, MinimumLength =2)]
        public string? LastName { get; set; }

        [Required(ErrorMessage ="La dirección es obligatoria")]
        [StringLength(255)]
        public string? Adress { get; set; }

        [Required(ErrorMessage ="El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage ="Formato de número de teléfono invalido.")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\d{8}-\d$", ErrorMessage ="Formato: 12345678-9")]
        [StringLength(10)]
        public string? Dui { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
