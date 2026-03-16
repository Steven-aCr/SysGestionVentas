using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required(ErrorMessage ="El tipo de documento es obligatorio.")]
        [ForeignKey("DocumentType")]
        public int DocTypeId { get; set; }
        public DocumentType? DocumentType { get; set; }

        [Required(ErrorMessage ="El número de documento es obligatorio.")]
        [StringLength(100, MinimumLength = 30)]
        public string DocNumber { get; set; } = string.Empty;

        [Required(ErrorMessage ="La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage ="La persona es obligatoria.")]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }
    }
}
