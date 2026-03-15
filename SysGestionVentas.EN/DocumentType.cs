

using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class DocumentType
    {
        [Key]
        public int DocTypeId { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 6)]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
