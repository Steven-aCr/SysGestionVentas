using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.EN
{
    public class RolPermission
    {
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}
