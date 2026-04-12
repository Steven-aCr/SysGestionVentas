using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysGestionVentas.EN
{
    /// <summary>
    /// Entidad de unión que representa la relación muchos a muchos
    /// entre <see cref="Rol"/> y <see cref="Permission"/>.
    /// Define qué permisos tiene asignado cada rol del sistema.
    /// </summary>
    public class RolPermission
    {
        [Required]
        public int RolId { get; set; }
        public Rol? Rol { get; set; }

        [Required]
        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("AssignedBy")]
        public int AssignedByUser { get; set; }
        public User? AssignedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}