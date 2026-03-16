using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN
{
    public class DocumentType
    {
        [Key]
        public int DocTypeId { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
