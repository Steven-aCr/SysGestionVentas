using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class Client
    {
        public string name { get; set; }    
        [Key]
        public int ClientId { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set;  }

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public int Id { get; set; }

        // Provide PascalCase properties used by the views. These are computed and not mapped to the database.
        [NotMapped]
        public string Name => !string.IsNullOrEmpty(name) ? name : (Person != null ? Person.FullName : string.Empty);

        [NotMapped]
        public string Email => Person?.Email ?? string.Empty;

        public int DocumentTypeId { get; set; }
    }
}
