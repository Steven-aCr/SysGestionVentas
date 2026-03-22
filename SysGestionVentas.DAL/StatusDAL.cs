using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class StatusDAL
    {
        public static async Task<List<Status>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.Status.ToListAsync();
        }
    }
}
