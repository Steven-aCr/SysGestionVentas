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
        /// <summary>
        /// Método para registrar un movimiento de inventario
        /// </summary>
        /// <param name="pInventoryMovement">Entidad que contiene los datos del movimiento a registrar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Registra el movimiento y retorna el número de filas afectadas</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar registrar</exception>
        public static async Task<int> RegistrarMovimientoAsync(InventoryMovement pInventoryMovement, DbContexto dbContexto)
        {
            try
            {
                // Se especifica el DbSet (InventoryMovement) al que se agregará la entidad
                dbContexto.InventoryMovement.Add(pInventoryMovement);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al registrar el movimiento de inventario.", ex);
            }
        }

        /// <summary>
        /// Método para consultar el historial de movimientos de un producto específico
        /// </summary>
        /// <param name="idProducto">El ID del producto a consultar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista de movimientos ordenados por fecha de registro descendente</returns>
        public static async Task<List<InventoryMovement>> ConsultarPorProductoAsync(int idProducto, DbContexto dbContexto)
        {
            return await dbContexto.InventoryMovement
                .Where(m => m.IdProduct == idProducto)
                .OrderByDescending(m => m.RegistrationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Método para obtener todos los movimientos de inventario registrados
        /// </summary>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista completa de los movimientos</returns>
        public static async Task<List<InventoryMovement>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.InventoryMovement.ToListAsync();
        }

        /// <summary>
        /// Método para obtener un movimiento específico por su ID
        /// </summary>
        /// <param name="id">El identificador único del movimiento</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna la entidad del movimiento si la encuentra, o null si no existe</returns>
        public static async Task<InventoryMovement> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.InventoryMovement.FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Método para modificar la información de un movimiento de inventario existente
        /// </summary>
        /// <param name="pInventoryMovement">Entidad que contiene los datos actualizados</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar modificar</exception>
        public static async Task<int> ModificarAsync(InventoryMovement pInventoryMovement, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que actualice la entidad
                dbContexto.InventoryMovement.Update(pInventoryMovement);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al modificar el movimiento de inventario.", ex);
            }
        }

        /// <summary>
        /// Método para eliminar un movimiento de inventario de la base de datos
        /// </summary>
        /// <param name="pInventoryMovement">Entidad del movimiento que se desea eliminar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar eliminar</exception>
        public static async Task<int> EliminarAsync(InventoryMovement pInventoryMovement, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que elimine la entidad
                dbContexto.InventoryMovement.Remove(pInventoryMovement);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el movimiento de inventario.", ex);
            }
        }
    }
}