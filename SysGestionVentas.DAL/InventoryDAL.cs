using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.DAL
{
    public class InventoryDAL
    {
        /// <summary>
        /// Aplica los filtros y el ordenamiento a la consulta base de inventario.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pQuery">Consulta base sobre la entidad <see cref="Inventory"/>.</param>
        /// <param name="pagedQuery">
        /// Objeto <see cref="PagedQuery{Inventory}"/> que contiene el filtro con:
        /// <list type="bullet">
        ///   <item><description><c>InventoryId</c>: filtra por ID de inventario (0 = sin filtro).</description></item>
        ///   <item><description><c>ProductId</c>: filtra por ID de producto asociado (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// <see cref="IQueryable{Inventory}"/> con los filtros aplicados,
        /// ordenado por <c>InventoryId</c> de forma ascendente.
        /// </returns>
        private static IQueryable<Inventory> QuerySelect(IQueryable<Inventory> pQuery, PagedQuery<Inventory> pagedQuery)
        {
            var F = pagedQuery.Filter;

            if (F.InventoryId > 0)
                pQuery = pQuery.Where(i => i.InventoryId == F.InventoryId);

            if (F.ProductId > 0)
                pQuery = pQuery.Where(i => i.ProductId == F.ProductId);

            return pQuery.OrderBy(i => i.InventoryId);
        }

        /// <summary>
        /// Obtiene una lista paginada de registros de inventario aplicando los filtros
        /// definidos en <paramref name="pagedQuery"/>.
        /// </summary>
        /// <param name="pagedQuery">
        /// Objeto <see cref="PagedQuery{Inventory}"/> con los parámetros de filtro y paginación:
        /// <list type="bullet">
        ///   <item><description><c>Filter</c>: criterios de búsqueda opcionales.</description></item>
        ///   <item><description><c>Top</c>: si es mayor a 0, limita directamente los resultados sin paginar.</description></item>
        ///   <item><description><c>Skip</c> / <c>PageSize</c>: para paginación estándar.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Inventory}"/> con los items encontrados,
        /// el total de registros, la página actual y el tamaño de página.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<PagedResult<Inventory>> BuscarAsync(PagedQuery<Inventory> pagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Inventory
                        .Include(i => i.Product)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pagedQuery);

                    int total = await filtered.CountAsync();

                    List<Inventory> items;

                    if (pagedQuery.Top > 0)
                    {
                        // Modo Top: retorna solo los primeros N registros sin paginación estándar.
                        items = await filtered
                            .Take(pagedQuery.Top)
                            .ToListAsync();
                    }
                    else
                    {
                        // Modo paginado: aplica salto y tamaño de página.
                        items = await filtered
                            .Skip(pagedQuery.Skip)
                            .Take(pagedQuery.PageSize)
                            .ToListAsync();
                    }

                    return new PagedResult<Inventory>
                    {
                        Items = items,
                        TotalCount = total,
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
        /// Registra un nuevo registro de inventario en la base de datos.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Inventory pInventory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    pInventory.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pInventory);
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
        /// Modifica los datos de un registro de inventario existente en la base de datos.
        /// No permite cambiar el producto asociado (<c>ProductId</c>) ya que es
        /// un campo estructural definido en la creación del registro.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el inventario no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Inventory pInventory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var inventory = await dbContexto.Inventory.FirstOrDefaultAsync(
                        i => i.InventoryId == pInventory.InventoryId);

                    if (inventory == null)
                        throw new Exception($"No se encontró el inventario con ID {pInventory.InventoryId}.");

                    inventory.PurchasePrice = pInventory.PurchasePrice;
                    inventory.SalePrice = pInventory.SalePrice;
                    inventory.MinimumStock = pInventory.MinimumStock;
                    inventory.CurrentStock = pInventory.CurrentStock;
                    inventory.StatusId = pInventory.StatusId;

                    dbContexto.Update(inventory);
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
        /// Realiza una eliminación lógica de un registro de inventario,
        /// cambiando su estado en la base de datos. No elimina el registro físicamente.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el inventario no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Inventory pInventory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var inventory = await dbContexto.Inventory.FirstOrDefaultAsync(
                        i => i.InventoryId == pInventory.InventoryId);

                    if (inventory == null)
                        throw new Exception($"No se encontró el inventario con ID {pInventory.InventoryId}.");

                    // Eliminación lógica: se cambia el estado del inventario
                    // en lugar de eliminarlo físicamente de la base de datos.
                    inventory.StatusId = pInventory.StatusId;

                    dbContexto.Update(inventory);
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
        /// Obtiene un registro de inventario específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Product"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con el <c>InventoryId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Inventory"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Inventory?> ObtenerPorIdAsync(Inventory pInventory)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Inventory
                        .Include(i => i.Product)
                        .Include(i => i.Status)
                        .FirstOrDefaultAsync(i => i.InventoryId == pInventory.InventoryId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de registros de inventario aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>ProductId</c>: filtra por producto asociado (0 = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Inventory"/> que cumplen los filtros indicados,
        /// ordenados por nombre de producto de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Inventory>> ObtenerTodosAsync(Inventory pInventory)
        {
            var result = new List<Inventory>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Inventory
                        .Include(i => i.Product)
                        .Include(i => i.Status)
                        .Where(i =>
                            (pInventory.ProductId == 0 || i.ProductId == pInventory.ProductId) &&
                            (pInventory.StatusId == 0 || i.StatusId == pInventory.StatusId)
                        )
                        .OrderBy(i => i.Product!.Name)
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