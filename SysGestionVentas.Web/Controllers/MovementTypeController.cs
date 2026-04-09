using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class MovementTypeController : Controller
    {
        private readonly MovementTypeBL _movementTypeBL;

        // Constructor con inyección de dependencias
        public MovementTypeController(MovementTypeBL movementTypeBL)
        {
            _movementTypeBL = movementTypeBL ?? throw new ArgumentNullException(nameof(movementTypeBL));
        }

        // GET: MovementType
        public async Task<IActionResult> Index()
        {
            try
            {
                var movementTypes = await _movementTypeBL.ObtenerTodosAsync();
                return View(movementTypes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<MovementType>());
            }
        }

        // GET: MovementType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var movementType = await _movementTypeBL.ObtenerPorIdAsync(id.Value);
                if (movementType == null)
                    return NotFound();
                return View(movementType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: MovementType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MovementType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovementType movementType)
        {
            if (!ModelState.IsValid)
                return View(movementType);
            try
            {
                await _movementTypeBL.GuardarAsync(movementType);
                TempData["Success"] = "Tipo de movimiento creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(movementType);
            }
        }

        // GET: MovementType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var movementType = await _movementTypeBL.ObtenerPorIdAsync(id.Value);
                if (movementType == null)
                    return NotFound();
                return View(movementType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MovementType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovementType movementType)
        {
            if (id != movementType.MovementTypeId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(movementType);
            try
            {
                await _movementTypeBL.ModificarAsync(movementType);
                TempData["Success"] = "Tipo de movimiento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(movementType);
            }
        }

        // GET: MovementType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var movementType = await _movementTypeBL.ObtenerPorIdAsync(id.Value);
                if (movementType == null)
                    return NotFound();
                return View(movementType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MovementType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _movementTypeBL.EliminarAsync(id);
                TempData["Success"] = "Tipo de movimiento eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

