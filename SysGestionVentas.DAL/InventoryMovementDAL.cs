using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class InventoryMovementDAL
    {
        public static async Task<int> RegistrarMovimientoAsync(InventoryMovement movement, DbContexto dbContexto)
        {
            dbContexto.Add(movement);
            return await dbContexto.SaveChangesAsync();
        }

        public static async Task<List<InventoryMovement>> ConsultarPorProductoAsync(int idProducto, DbContexto dbContexto)
        {
            return await dbContexto.InventoryMovement
                .Where(m => m.IdProduct == idProducto)
                .OrderByDescending(m => m.RegistrationDate)
                .ToListAsync();
        }
    }
}