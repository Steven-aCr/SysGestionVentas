using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int PersonId { get; set; }

    }
}
