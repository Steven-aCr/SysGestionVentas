using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace SysGestionVentas.DAL
{
    public class ClientDAL
    {

        private static IQueryable<Client> QuerySelect(IQueryable<Client> pQuery, PagedQuery<Client> pagedQuery)
        {
            var F = pagedQuery.Filter;

            if (F.ClientId > 0)
                pQuery = pQuery.Where(c => c.ClientId == F.ClientId);
            if (F.PersonId > 0)
                pQuery = pQuery.Where(c => c.PersonId == F.PersonId);
            return pQuery.OrderBy(c => c.ClientId);
        }

        public static async Task<PagedResult<Client>> BuscarAsync(PagedQuery<Client> pagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Client
                        .Include(c => c.Person)
                        .AsQueryable();
                    var Filtered = QuerySelect(baseQuery, pagedQuery);
                    int Total = await Filtered.CountAsync();
                    List<Client> items;
                    if (pagedQuery.Top < 0)
                    {
                        items = await Filtered
                            .Take(pagedQuery.Top)
                            .ToListAsync();
                    }
                    else
                    {
                        items = await Filtered
                            .Skip(pagedQuery.Skip)
                            .Take(pagedQuery.PageSize)
                            .ToListAsync();
                    }
                    return new PagedResult<Client>
                    {
                        Items = items,
                        TotalCount = Total,
                        CurrentPage = pagedQuery.Page,
                        PageSize = pagedQuery.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }



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
        /// Realiza una eliminación lógica de un cliente, cambiando el estado de su
        /// <see cref="Person"/> asociada en la base de datos. No elimina el registro físicamente.
        /// La entidad <see cref="Client"/> no posee <c>StatusId</c> propio; el estado
        /// se gestiona a través de <see cref="Person.StatusId"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con el <c>ClientId</c> del registro a desactivar.
        /// Debe incluir <c>Person.StatusId</c> con el estado inactivo a aplicar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el cliente o su persona asociada no existen,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var client = await dbContexto.Client
                        .Include(c => c.Person)
                        .FirstOrDefaultAsync(c => c.ClientId == pClient.ClientId);

                    if (client == null)
                        throw new Exception($"No se encontró el cliente con ID {pClient.ClientId}.");

                    if (client.Person == null)
                        throw new Exception($"No se encontró la persona asociada al cliente con ID {pClient.ClientId}.");

                    // Eliminación lógica: se cambia el estado de la Person asociada
                    // ya que Client no posee StatusId propio.
                    client.Person.StatusId = pClient.Person!.StatusId;

                    dbContexto.Update(client.Person);
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
        /// su relación con <see cref="Person"/> y el <see cref="Status"/> de la persona.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> con el <c>ClientId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Client"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Client?> ObtenerPorIdAsync(Client pClient)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Client
                        .Include(c => c.Person)
                            .ThenInclude(p => p!.Status)
                        .FirstOrDefaultAsync(c => c.ClientId == pClient.ClientId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de clientes aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> o <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>PersonId</c>: filtra por persona asociada (0 = sin filtro).</description></item>
        ///   <item><description><c>Person.StatusId</c>: filtra por estado de la persona (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Client"/> que cumplen los filtros indicados,
        /// ordenados por apellido de la persona asociada de forma ascendente.
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
                            .ThenInclude(p => p!.Status)
                        .Where(c =>
                            (pClient.PersonId == 0 || c.PersonId == pClient.PersonId) &&
                            (pClient.Person!.StatusId == 0 || c.Person!.StatusId == pClient.Person.StatusId)
                        )
                        .OrderBy(c => c.Person!.LastName)
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