using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierBL _supplierBL;

        // Constructor con inyección de dependencias
        public SupplierController(SupplierBL supplierBL)
        {
            _supplierBL = supplierBL ?? throw new ArgumentNullException(nameof(supplierBL));
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            try
            {
                var suppliers = await _supplierBL.ObtenerTodosAsync();
                return View(suppliers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Supplier>());
            }
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var supplier = await _supplierBL.ObtenerPorIdAsync(id.Value);
                if (supplier == null)
                    return NotFound();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier pSupplier)
        {
            if (!ModelState.IsValid)
                return View(pSupplier);
            try
            {
                await _supplierBL.GuardarAsync(pSupplier);
                TempData["Success"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pSupplier);
            }
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var supplier = await _supplierBL.ObtenerPorIdAsync(id.Value);
                if (supplier == null)
                    return NotFound();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier pSupplier)
        {
            if (id != pSupplier.SupplierId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(pSupplier);
            try
            {
                await _supplierBL.ModificarAsync(pSupplier);
                TempData["Success"] = "Proveedor modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pSupplier);
            }
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var supplier = await _supplierBL.ObtenerPorIdAsync(id.Value);
                if (supplier == null)
                    return NotFound();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _supplierBL.EliminarAsync(id);
                TempData["Success"] = "Proveedor eliminado correctamente.";
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
