using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El código del empleado es obligatorio")]
        [StringLength(50)]
        public string? EmployeeCode { get; set; }

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public int PersonId { get; set; }

        public int StatusId { get; set; }

    }
}
