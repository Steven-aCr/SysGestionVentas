using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.DAL
{
    public class InventoryMovementDAL
    {
        #region "CRUD"
              
        /// <summary>
        /// Registra un nuevo movimiento de inventario en la base de datos
        /// creando su propio contexto de base de datos internamente.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> con los datos del movimiento a registrar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se registró correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> RegistrarMovimientoAsync(InventoryMovement pInventoryMovement)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    pInventoryMovement.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pInventoryMovement);
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
        /// Obtiene un movimiento de inventario específico por su identificador, incluyendo
        /// sus relaciones con <see cref="MovementType"/>, <see cref="Inventory"/> y
        /// el <see cref="User"/> que lo registró.
        /// </summary>
        /// <remarks>
        /// Los movimientos de inventario son registros históricos inmutables.
        /// Este método es de solo lectura; no existe modificación ni eliminación de movimientos.
        /// </remarks>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> con el <c>InventoryMovementId</c> a buscar.
        /// </param>
        /// <returns>
        /// El objeto <see cref="InventoryMovement"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<InventoryMovement?> ObtenerPorIdAsync(InventoryMovement pInventoryMovement)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.InventoryMovement
                        .Include(im => im.MovementType)
                        .Include(im => im.Inventory)
                            .ThenInclude(i => i!.Product)
                        .Include(im => im.CreatedBy)
                        .FirstOrDefaultAsync(im => im.InventoryMovementId == pInventoryMovement.InventoryMovementId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de movimientos de inventario aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> o <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>InventoryId</c>: filtra por inventario asociado (0 = sin filtro).</description></item>
        ///   <item><description><c>MovementTypeId</c>: filtra por tipo de movimiento (0 = sin filtro).</description></item>
        ///   <item><description><c>CreatedByUser</c>: filtra por usuario que registró el movimiento (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <param name="pFromDate">Fecha de inicio del rango de búsqueda (null = sin límite inferior).</param>
        /// <param name="pToDate">Fecha de fin del rango de búsqueda (null = sin límite superior).</param>
        /// <returns>
        /// Lista de objetos <see cref="InventoryMovement"/> que cumplen los filtros indicados,
        /// ordenados por fecha de creación de forma descendente (más recientes primero).
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<InventoryMovement>> ObtenerTodosAsync(
            InventoryMovement pInventoryMovement,
            DateTime? pFromDate = null,
            DateTime? pToDate = null)
        {
            var result = new List<InventoryMovement>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.InventoryMovement
                        .Include(im => im.MovementType)
                        .Include(im => im.Inventory)
                            .ThenInclude(i => i!.Product)
                        .Include(im => im.CreatedBy)
                        .Where(im =>
                            (pInventoryMovement.InventoryId == 0 || im.InventoryId == pInventoryMovement.InventoryId) &&
                            (pInventoryMovement.MovementTypeId == 0 || im.MovementTypeId == pInventoryMovement.MovementTypeId) &&
                            (pInventoryMovement.CreatedByUser == 0 || im.CreatedByUser == pInventoryMovement.CreatedByUser) &&
                            (pFromDate == null || im.CreatedAt >= pFromDate.Value) &&
                            (pToDate == null || im.CreatedAt <= pToDate.Value)
                        )
                        .OrderByDescending(im => im.CreatedAt)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene el historial completo de movimientos asociados a un inventario específico,
        /// ordenados cronológicamente de forma descendente.
        /// </summary>
        /// <param name="pInventoryId">Identificador del inventario a consultar.</param>
        /// <returns>
        /// Lista de objetos <see cref="InventoryMovement"/> del inventario indicado,
        /// con <see cref="MovementType"/> y <see cref="User"/> creador cargados.
        /// Retorna lista vacía si no existen movimientos.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<InventoryMovement>> ObtenerPorInventarioAsync(int pInventoryId)
        {
            var result = new List<InventoryMovement>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.InventoryMovement
                        .Include(im => im.MovementType)
                        .Include(im => im.CreatedBy)
                        .Where(im => im.InventoryId == pInventoryId)
                        .OrderByDescending(im => im.CreatedAt)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        #endregion

/// <summary>
/// Obtiene una lista de movimientos de inventario filtrada por fechas y con paginación.
/// </summary>
/// <param name="pagedQuery">Objeto PagedQuery que contiene los filtros de fecha, página y el filtro de InventoryMovement.</param>
/// <returns>Lista paginada de Movimientos de Inventario.</returns>
public static async Task<List<InventoryMovement>> ObtenerPaginadoYFiltradoAsync(PagedQuery<InventoryMovement> pagedQuery)
{
    var result = new List<InventoryMovement>();
    try
    {
        using (var dbContexto = new DbContexto())
        {
            // Empezamos la consulta incluyendo las relaciones para que devuelva el Producto, Tipo y Estado
            var query = dbContexto.InventoryMovement
                .Include(i => i.Product)
                .Include(i => i.MovementType)
                .Include(i => i.Status)
                .AsQueryable();

            // 1. Aplicar los filtros normales si vienen en pagedQuery.Filter
            if (pagedQuery.Filter != null)
            {
                if (pagedQuery.Filter.ProductId > 0)
                    query = query.Where(i => i.ProductId == pagedQuery.Filter.ProductId);

                if (pagedQuery.Filter.MovementTypeId > 0)
                    query = query.Where(i => i.MovementTypeId == pagedQuery.Filter.MovementTypeId);

                if (pagedQuery.Filter.StatusId > 0)
                    query = query.Where(i => i.StatusId == pagedQuery.Filter.StatusId);
            }

            // 2. Aplicar el FILTRO DE FECHAS (Usando tu propiedad MovementDate)
            if (pagedQuery.FromDate.HasValue)
                query = query.Where(i => i.MovementDate >= pagedQuery.FromDate.Value);

            if (pagedQuery.ToDate.HasValue)
                query = query.Where(i => i.MovementDate <= pagedQuery.ToDate.Value);

            // 3. Aplicar Paginación (para no saturar la memoria)
            int skip = (pagedQuery.Page - 1) * pagedQuery.PageSize;

            // Ordenamos por fecha del movimiento de la más reciente a la más antigua
            result = await query.OrderByDescending(i => i.MovementDate)
                                .Skip(skip)
                                .Take(pagedQuery.PageSize)
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