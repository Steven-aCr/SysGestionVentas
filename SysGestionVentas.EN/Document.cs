using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SysGestionVentas.EN
{
    public class Document
    {
        [Key]

        public int DocumentId { get; set; }
        [Required]
        [ForeignKey("DocumentType")]
        public int DocTypeId { get; set; }
        public DocumentType? DocumentType { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 30)]

        public string DocNumber { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }
        [Required]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person? Person { get; set; }
    }
}
