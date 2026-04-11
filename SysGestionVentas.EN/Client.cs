using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [ForeignKey("Person")]
        public int? PersonId { get; set; }
        public Person? Person { get; set; }

        public string? Address { get; set; }

        [ForeignKey("Status")]
        [NotMapped]
        public string StatusDescription => StatusId switch
        {
            1 => "Activo",
            2 => "Inactivo",
            3 => "Bloqueado",
            _ => "Desconocido" // Valor por defecto si no coincide
        };
        public int? StatusId { get; set; }
      
       
        public Status? Status { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public int NumberPhone { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Name { get; set; }

        [NotMapped]
        public string FullNameDisplay => Name ?? (Person != null ? $"{Person.FirstName} {Person.LastName}" : "Sin Nombre");

        [NotMapped]
        public string Email => Person?.Email ?? "N/A";
    }
}