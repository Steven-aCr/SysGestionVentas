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
        [Key]
        public int ClientId { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set;  }

    }
}
