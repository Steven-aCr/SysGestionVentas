using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Método para guardar información de un nuevo Tipo de Estatus
        /// </summary>
        /// <param name="pStatusType">Entidad que contiene los datos del tipo de estatus a guardar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de registros afectados en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar guardar</exception>
        public static async Task<int> GuardarAsync(StatusType pStatusType, DbContexto dbContexto)
        {
            try
            {
                // Se agrega la entidad al contexto indicando el DbSet correspondiente
                dbContexto.StatusType.Add(pStatusType);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar el tipo de estatus.", ex);
            }
        }

        /// <summary>
        /// Método para obtener todos los tipos de estatus registrados
        /// </summary>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista completa de los tipos de estatus</returns>
        public static async Task<List<StatusType>> ObtenerTodosAsync(DbContexto dbContexto)
        {
            return await dbContexto.StatusType.ToListAsync();
        }

        /// <summary>
        /// Método para obtener un tipo de estatus específico por su ID
        /// </summary>
        /// <param name="id">El identificador único del tipo de estatus</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna la entidad del tipo de estatus si la encuentra, o null si no existe</returns>
        public static async Task<StatusType> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.StatusType.FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Método para modificar la información de un tipo de estatus existente
        /// </summary>
        /// <param name="pStatusType">Entidad que contiene los datos actualizados del tipo de estatus</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar modificar</exception>
        public static async Task<int> ModificarAsync(StatusType pStatusType, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que actualice la entidad
                dbContexto.StatusType.Update(pStatusType);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al modificar el tipo de estatus.", ex);
            }
        }

        /// <summary>
        /// Método para eliminar un tipo de estatus de la base de datos
        /// </summary>
        /// <param name="pStatusType">Entidad del tipo de estatus que se desea eliminar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar eliminar</exception>
        public static async Task<int> EliminarAsync(StatusType pStatusType, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que elimine la entidad
                dbContexto.StatusType.Remove(pStatusType);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el tipo de estatus.", ex);
            }
        }
    }
}