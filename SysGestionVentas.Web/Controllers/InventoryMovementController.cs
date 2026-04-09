using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    public class InventoryMovementController : Controller
    {
        private readonly DbContexto _context;

        public InventoryMovementController(DbContexto context)
        {
            _context = context;
        }

        // GET: InventoryMovement
        public async Task<IActionResult> Index()
        {
            var movements = await _context.InventoryMovement
                .Include(m => m.Inventory)
                .ToListAsync();
            return View(movements);
        }

        // GET: InventoryMovement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var movement = await _context.InventoryMovement
                .Include(m => m.Inventory)
                .FirstOrDefaultAsync(m => m.InventoryMovementId == id);

            if (movement == null)
                return NotFound();

            return View(movement);
        }

        // GET: InventoryMovement/Create
        public IActionResult Create()
        {
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId");
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "UserName");
            return View();
        }

        // POST: InventoryMovement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryMovementId,MovementType,Quantity,UnitCost,CreatedByUser,InventoryId")] InventoryMovement pInventoryMovement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await InventoryMovementBL.GuardarAsync(pInventoryMovement);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", pInventoryMovement.InventoryId);
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "UserName", pInventoryMovement.CreatedByUser);
            return View(pInventoryMovement);
        }

        // GET: InventoryMovement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var movement = await _context.InventoryMovement.FindAsync(id);
            if (movement == null)
                return NotFound();

            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", movement.InventoryId);
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "UserName", movement.CreatedByUser);
            return View(movement);
        }

        // POST: InventoryMovement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryMovementId,MovementType,Quantity,UnitCost,CreatedByUser,InventoryId")] InventoryMovement pInventoryMovement)
        {
            if (id != pInventoryMovement.InventoryMovementId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await InventoryMovementBL.ModificarAsync(pInventoryMovement);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", pInventoryMovement.InventoryId);
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "UserName", pInventoryMovement.CreatedByUser);
            return View(pInventoryMovement);
        }

        // GET: InventoryMovement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var movement = await _context.InventoryMovement
                .Include(m => m.Inventory)
                .FirstOrDefaultAsync(m => m.InventoryMovementId == id);

            if (movement == null)
                return NotFound();

            return View(movement);
        }

        // POST: InventoryMovement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movement = new InventoryMovement { InventoryMovementId = id };
            try
            {
                await InventoryMovementBL.EliminarAsync(movement);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryMovementExists(int id)
        {
            return _context.InventoryMovement.Any(m => m.InventoryMovementId == id);
        }
    }
}