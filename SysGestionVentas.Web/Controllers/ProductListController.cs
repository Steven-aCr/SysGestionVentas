using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class ProductListController : Controller
    {
        private readonly ProductListBL _productListBL;

        // Constructor con inyección de dependencias
        public ProductListController(ProductListBL productListBL)
        {
            _productListBL = productListBL ?? throw new ArgumentNullException(nameof(productListBL));
        }

        // GET: ProductList
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productListBL.ObtenerTodosAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<ProductList>());
            }
        }

        // GET: ProductList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await _productListBL.ObtenerPorIdAsync(id.Value);
                if (product == null)
                    return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ProductList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductList product)
        {
            if (!ModelState.IsValid)
                return View(product);
            try
            {
                await _productListBL.GuardarAsync(product);
                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(product);
            }
        }

        // GET: ProductList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await _productListBL.ObtenerPorIdAsync(id.Value);
                if (product == null)
                    return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductList product)
        {
            if (id != product.ProductId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(product);
            try
            {
                await _productListBL.ModificarAsync(product);
                TempData["Success"] = "Producto modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(product);
            }
        }

        // GET: ProductList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await _productListBL.ObtenerPorIdAsync(id.Value);
                if (product == null)
                    return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productListBL.EliminarAsync(id);
                TempData["Success"] = "Producto eliminado correctamente.";
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
