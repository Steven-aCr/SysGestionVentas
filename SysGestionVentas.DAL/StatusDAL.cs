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
                // Se agrega la entidad al contexto
                dbContexto.Status.Add(pStatus);

                // Se guardan los cambios de forma asíncrona y se retorna el número de filas afectadas
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Se captura y relanza la excepción en caso de error
                throw new Exception("Ocurrió un error al guardar el status.", ex);
            }
        }
    }
}