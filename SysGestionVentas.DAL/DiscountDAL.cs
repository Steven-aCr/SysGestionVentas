using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class DiscountDAL
    {
        public static async Task<int> GuardarAsync(Discount discount, DbContexto dbContexto)
        {
            dbContexto.Add(discount);
            return await dbContexto.SaveChangesAsync();
        }

        public static async Task<Discount> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.Discount.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}