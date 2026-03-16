using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SysGestionVentas.EN
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El código del empleado es obligatorio")]
        [StringLength(50, MinimumLength =3,
            ErrorMessage ="El código de empleado debe tener 3 y 50 caracteres.")]
        public string? EmployeeCode { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage ="El salario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage ="El salario debe ser mayor a $0.00")]
        [Column(TypeName ="Decimal(18,2)")]
        public decimal Salary { get; set; }

        //[StringLength(100)]
        //public string? Department { get; set; }

        //[ForeignKey("User")]
        //public int? UserId { get; set; }
        //public User? User { get; set; }

        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

    }
}
