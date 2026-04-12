using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryBL _categoryBL;

        // Constructor con inyección de dependencias
        public CategoryController(CategoryBL categoryBL)
        {
            _categoryBL = categoryBL ?? throw new ArgumentNullException(nameof(categoryBL));
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryBL.ObtenerTodosAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Category>());
            }
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var category = await _categoryBL.ObtenerPorIdAsync(id.Value);
                if (category == null)
                    return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);
            try
            {
                await _categoryBL.GuardarAsync(category);
                TempData["Success"] = "Categoría creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(category);
            }
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var category = await _categoryBL.ObtenerPorIdAsync(id.Value);
                if (category == null)
                    return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(category);
            try
            {
                await _categoryBL.ModificarAsync(category);
                TempData["Success"] = "Categoría modificada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(category);
            }
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var category = await _categoryBL.ObtenerPorIdAsync(id.Value);
                if (category == null)
                    return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryBL.EliminarAsync(id);
                TempData["Success"] = "Categoría eliminada correctamente.";
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
