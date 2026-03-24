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
        /// Método para guardar información de un nuevo descuento
        /// </summary>
        /// <param name="pDiscount">Entidad que contiene los datos del descuento a guardar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas</returns>
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

        /// <summary>
        /// Método para obtener un descuento específico por su ID
        /// </summary>
        /// <param name="id">El identificador único del descuento</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna la entidad del descuento si la encuentra, o null si no existe</returns>
        public static async Task<Discount> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.Discount.FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Método para obtener todos los descuentos registrados
        /// </summary>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista completa de descuentos</returns>
        public static async Task<List<Discount>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.Discount.ToListAsync();
        }

        /// <summary>
        /// Método para modificar la información de un descuento existente
        /// </summary>
        /// <param name="pDiscount">Entidad que contiene los datos actualizados del descuento</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar modificar</exception>
        public static async Task<int> ModificarAsync(Discount pDiscount, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que actualice la entidad
                dbContexto.Discount.Update(pDiscount);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al modificar el descuento.", ex);
            }
        }

        /// <summary>
        /// Método para eliminar un descuento de la base de datos
        /// </summary>
        /// <param name="pDiscount">Entidad del descuento que se desea eliminar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar eliminar</exception>
        public static async Task<int> EliminarAsync(Discount pDiscount, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que elimine la entidad
                dbContexto.Discount.Remove(pDiscount);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el descuento.", ex);
            }
        }
    }
}