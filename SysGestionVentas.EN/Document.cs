using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [ForeignKey("DocumentType")]
        public int DocTypeId { get; set; }
        public DocumentType? DocumentType { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El número de documento debe tener entre 3 y 50 caracteres.")]
        public string? DocNumber { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto total debe ser mayor o igual a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } = 0;

        [Required(ErrorMessage = "La persona es obligatoria.")]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [ForeignKey("CreatedBy")]
        public int CreatedByUser { get; set; }
        public User? CreatedBy { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}