
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace BDGestionVentas.DAL
{
    public class InventoryDAL
    {
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
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Inventory pInventory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var inventory = await dbContexto.Inventory.FirstOrDefaultAsync(
                        i => i.InventoryId == pInventory.InventoryId);

                    inventory.PurchasePrice = pInventory.PurchasePrice;
                    inventory.SalePrice = pInventory.SalePrice;
                    inventory.MinimumStock = pInventory.MinimumStock;
                    inventory.CurrentStock = pInventory.CurrentStock;
                    inventory.ProductId = pInventory.ProductId;

                    dbContexto.Update(inventory);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Inventory pInventory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var inventory = await dbContexto.Inventory.FirstOrDefaultAsync(
                        i => i.InventoryId == pInventory.InventoryId);

                    dbContexto.Remove(inventory);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

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
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

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
                            (pInventory.ProductId == 0 || i.ProductId == pInventory.ProductId) &&
                            (pInventory.CurrentStock == 0 || i.CurrentStock <= pInventory.CurrentStock)
                        )
                        .OrderBy(i => i.ProductId)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}