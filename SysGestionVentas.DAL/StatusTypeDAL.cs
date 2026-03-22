using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class StatusTypeDAL
    {
        public static async Task<List<StatusType>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.StatusType.ToListAsync();
        }
    }
}