using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador")]
    public class InventoryMovementsController : Controller
    {
        private readonly DbContexto _context;

        public InventoryMovementsController(DbContexto context)
        {
            _context = context;
        }

        // GET: InventoryMovements
        public async Task<IActionResult> Index()
        {
            var dbContexto = _context.InventoryMovement.Include(i => i.CreatedBy).Include(i => i.Inventory).Include(i => i.MovementType);
            return View(await dbContexto.ToListAsync());
        }

        // GET: InventoryMovements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryMovement = await _context.InventoryMovement
                .Include(i => i.CreatedBy)
                .Include(i => i.Inventory)
                .Include(i => i.MovementType)
                .FirstOrDefaultAsync(m => m.InventoryMovementId == id);
            if (inventoryMovement == null)
            {
                return NotFound();
            }

            return View(inventoryMovement);
        }

        // GET: InventoryMovements/Create
        public IActionResult Create()
        {
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email");
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId");
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Name");
            return View();
        }

        // POST: InventoryMovements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryMovementId,MovementTypeId,Quantity,UnitCost,Notes,CreatedAt,CreatedByUser,InventoryId,MovementDate")] InventoryMovement inventoryMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", inventoryMovement.CreatedByUser);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", inventoryMovement.InventoryId);
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Name", inventoryMovement.MovementTypeId);
            return View(inventoryMovement);
        }

        // GET: InventoryMovements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryMovement = await _context.InventoryMovement.FindAsync(id);
            if (inventoryMovement == null)
            {
                return NotFound();
            }
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", inventoryMovement.CreatedByUser);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", inventoryMovement.InventoryId);
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Name", inventoryMovement.MovementTypeId);
            return View(inventoryMovement);
        }

        // POST: InventoryMovements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryMovementId,MovementTypeId,Quantity,UnitCost,Notes,CreatedAt,CreatedByUser,InventoryId,MovementDate")] InventoryMovement inventoryMovement)
        {
            if (id != inventoryMovement.InventoryMovementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryMovementExists(inventoryMovement.InventoryMovementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", inventoryMovement.CreatedByUser);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", inventoryMovement.InventoryId);
            ViewData["MovementTypeId"] = new SelectList(_context.MovementType, "MovementTypeId", "Name", inventoryMovement.MovementTypeId);
            return View(inventoryMovement);
        }

        // GET: InventoryMovements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryMovement = await _context.InventoryMovement
                .Include(i => i.CreatedBy)
                .Include(i => i.Inventory)
                .Include(i => i.MovementType)
                .FirstOrDefaultAsync(m => m.InventoryMovementId == id);
            if (inventoryMovement == null)
            {
                return NotFound();
            }

            return View(inventoryMovement);
        }

        // POST: InventoryMovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryMovement = await _context.InventoryMovement.FindAsync(id);
            if (inventoryMovement != null)
            {
                _context.InventoryMovement.Remove(inventoryMovement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryMovementExists(int id)
        {
            return _context.InventoryMovement.Any(e => e.InventoryMovementId == id);
        }

        private async Task CargarListasAsync()
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name");
        }
    }
}
