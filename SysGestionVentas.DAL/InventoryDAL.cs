using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class InventoryDAL
    {
        /// <summary>
        /// Registra un nuevo inventario en la base de datos.
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
                    pInventory.CreatedAt = DateTime.Now;
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
        /// Modifica los datos de un inventario existente en la base de datos.
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
                    inventory.ProductId = pInventory.ProductId;

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
        /// Realiza una eliminación lógica de un inventario, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado "inactivo/eliminado".
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
        /// Obtiene un inventario específico por su identificador, incluyendo
        /// su relación con <see cref="ProductList"/>.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con el <c>InventoryId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Inventory"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Inventory> ObtenerPorIdAsync(Inventory pInventory)
        {
            var result = new Inventory();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Inventory
                        .Include(i => i.ProductList)
                        .FirstOrDefaultAsync(i => i.InventoryId == pInventory.InventoryId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de inventarios aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>ProductId</c>: filtra por producto asociado (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Inventory"/> que cumplen los filtros indicados,
        /// ordenados por producto de forma ascendente.
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
                        .Include(i => i.ProductList)
                        .Where(i =>
                            (pInventory.ProductId == 0 || i.ProductId == pInventory.ProductId)
                        )
                        .OrderBy(i => i.ProductId)
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