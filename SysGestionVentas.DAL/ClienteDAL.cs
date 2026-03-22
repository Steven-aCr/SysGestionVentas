using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class ClientDAL
    {
        /// <summary>
        /// Registra un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pClient);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Modifica los datos de un cliente existente en la base de datos.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con el <c>ClientId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el cliente no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var client = await dbContexto.Client.FirstOrDefaultAsync(
                        c => c.ClientId == pClient.ClientId);

                    if (client == null)
                        throw new Exception($"No se encontró el cliente con ID {pClient.ClientId}.");

                    client.PersonId = pClient.PersonId;

                    dbContexto.Update(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Realiza una eliminación lógica de un cliente, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con el <c>ClientId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado "inactivo/eliminado".
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el cliente no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var client = await dbContexto.Client.FirstOrDefaultAsync(
                        c => c.ClientId == pClient.ClientId);

                    if (client == null)
                        throw new Exception($"No se encontró el cliente con ID {pClient.ClientId}.");

                    // Eliminación lógica: se cambia el estado del cliente
                    // en lugar de eliminarlo físicamente de la base de datos.
                    client.StatusId = pClient.StatusId;

                    dbContexto.Update(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene un cliente específico por su identificador, incluyendo
        /// su relación con <see cref="Person"/>.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> con el <c>ClientId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Client"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Client> ObtenerPorIdAsync(Client pClient)
        {
            var result = new Client();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Client
                        .Include(c => c.Person)
                        .FirstOrDefaultAsync(c => c.ClientId == pClient.ClientId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de clientes aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>PersonId</c>: filtra por persona asociada (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Client"/> que cumplen los filtros indicados,
        /// ordenados por apellido de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Client>> ObtenerTodosAsync(Client pClient)
        {
            var result = new List<Client>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Client
                        .Include(c => c.Person)
                        .Where(c =>
                            (pClient.PersonId == 0 || c.PersonId == pClient.PersonId)
                        )
                        .OrderBy(c => c.Person.LastName)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}