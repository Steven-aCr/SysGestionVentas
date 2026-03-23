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
        /// <summary>
        /// Método para guardar información del descuento
        /// </summary>
        /// <param name="pDiscount">Entidad que contiene los datos del descuento a guardar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Guarda los datos del descuento y retorna el número de filas afectadas</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar guardar</exception>
        public static async Task<int> GuardarAsync(Discount pDiscount, DbContexto dbContexto)
        {
            try
            {
                // Se especifica el DbSet (Discount) al que se agregará la entidad
                dbContexto.Discount.Add(pDiscount);

                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar el descuento.", ex);
            }
        }
    }
}