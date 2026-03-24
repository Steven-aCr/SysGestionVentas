using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Método para guardar información de un nuevo Status
        /// </summary>
        /// <param name="pStatus">Entidad que contiene los datos del status a guardar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de registros afectados en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar guardar el status</exception>
        public static async Task<int> GuardarAsync(Status pStatus, DbContexto dbContexto)
        {
            try
            {
                // Se agrega la entidad al contexto indicando el DbSet correspondiente
                dbContexto.Status.Add(pStatus);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar el status.", ex);
            }
        }

        /// <summary>
        /// Método para obtener todos los status registrados
        /// </summary>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista completa de los status</returns>
        public static async Task<List<Status>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.Status.ToListAsync();
        }

        /// <summary>
        /// Método para obtener un status específico por su ID
        /// </summary>
        /// <param name="id">El identificador único del status</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna la entidad del status si la encuentra, o null si no existe</returns>
        public static async Task<Status> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.Status.FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Método para modificar la información de un status existente
        /// </summary>
        /// <param name="pStatus">Entidad que contiene los datos actualizados del status</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar modificar</exception>
        public static async Task<int> ModificarAsync(Status pStatus, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que actualice la entidad
                dbContexto.Status.Update(pStatus);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al modificar el status.", ex);
            }
        }

        /// <summary>
        /// Método para eliminar un status de la base de datos
        /// </summary>
        /// <param name="pStatus">Entidad del status que se desea eliminar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar eliminar</exception>
        public static async Task<int> EliminarAsync(Status pStatus, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que elimine la entidad
                dbContexto.Status.Remove(pStatus);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el status.", ex);
            }
        }
    }
}