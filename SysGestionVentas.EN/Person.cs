using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [Required (ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage ="El apellido es obligatorio.")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required(ErrorMessage ="La dirección es obligatoria")]
        [StringLength(255)]
        public string? Adress { get; set; }

        [Required(ErrorMessage ="El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage ="Formato de número de teléfono invalido.")]
        public string? PhoneNumber { get; set; }

        [StringLength(10, ErrorMessage = "ingrese un número de DUI valido.")]
        public string? Dui { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
