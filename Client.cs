// C#
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; } // revisar nombre de columna en BD si falla
        public Status? Status { get; set; }

        // Campo backing privado en lugar de propiedad pública mapeada
        private string? _name;

        [NotMapped]
        public string Name => !string.IsNullOrEmpty(_name) ? _name : (Person?.FullName ?? string.Empty);

        [NotMapped]
        public string Email => Person?.Email ?? string.Empty;
    }
}