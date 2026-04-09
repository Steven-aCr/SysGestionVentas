using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly DbContexto _context;

        public InventoryController(DbContexto context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventory
                .Include(i => i.ProductList)
                .ToListAsync();
            return View(inventories);
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var inventory = await _context.Inventory
                .Include(i => i.ProductList)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // GET: Inventory/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name");
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,PurchasePrice,SalePrice,MinimumStock,CurrentStock,ProductId")] Inventory pInventory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await InventoryBL.GuardarAsync(pInventory);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", pInventory.ProductId);
            return View(pInventory);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
                return NotFound();

            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", inventory.ProductId);
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,PurchasePrice,SalePrice,MinimumStock,CurrentStock,ProductId")] Inventory pInventory)
        {
            if (id != pInventory.InventoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await InventoryBL.ModificarAsync(pInventory);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", pInventory.ProductId);
            return View(pInventory);
        }

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Deleet(int? id)
        {
            if (id == null)
                return NotFound();

            var inventory = await _context.Inventory
                .Include(i => i.ProductList)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = new Inventory
            {
                InventoryId = id,
                StatusId = 2 // Estado inactivo
            };

            try
            {
                await InventoryBL.EliminarAsync(inventory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(i => i.InventoryId == id);
        }
    }
}