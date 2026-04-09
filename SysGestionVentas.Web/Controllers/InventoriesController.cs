using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador,Vendedor")]
    public class InventoriesController : Controller
    {
        // GET: Inventories
        /// <summary>
        /// Muestra la lista paginada de registros de inventario con soporte de búsqueda.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="search">Texto de búsqueda por nombre de producto.</param>
        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            try
            {
                var query = new PagedQuery<Inventory>
                {
                    Filter = new Inventory { ProductId = 0, StatusId = 0 },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await InventoryBL.BuscarAsync(query);
                ViewBag.Search = search;
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Inventory>());
            }
        }

        // GET: Inventories/Details/5
        /// <summary>
        /// Muestra el detalle de un registro de inventario específico,
        /// incluyendo su historial de movimientos.
        /// </summary>
        /// <param name="id">Identificador del inventario a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var inventory = await InventoryBL.ObtenerPorIdAsync(new Inventory { InventoryId = id.Value });
                if (inventory == null) return NotFound();

                var movimientos = await InventoryMovementBL.ObtenerPorInventarioAsync(id.Value);
                ViewBag.Movimientos = movimientos;
                return View(inventory);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventories/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo inventario asociado a un producto.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Inventory());
        }

        // POST: Inventories/Create
        /// <summary>
        /// Procesa el registro de un nuevo inventario.
        /// Aplica validaciones de negocio antes de persistir.
        /// </summary>
        /// <param name="pInventory">Entidad <see cref="Inventory"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(
            [Bind("PurchasePrice,SalePrice,MinimumStock,CurrentStock,ProductId,StatusId")]
            Inventory pInventory)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pInventory);
            }

            try
            {
                await InventoryBL.GuardarAsync(pInventory);
                TempData["Success"] = "Inventario registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pInventory);
            }
        }

        // GET: Inventories/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un registro de inventario existente.
        /// </summary>
        /// <param name="id">Identificador del inventario a editar.</param>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var inventory = await InventoryBL.ObtenerPorIdAsync(new Inventory { InventoryId = id.Value });
                if (inventory == null) return NotFound();

                await CargarListasAsync();
                return View(inventory);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inventories/Edit/5
        /// <summary>
        /// Procesa la modificación de un registro de inventario existente.
        /// </summary>
        /// <param name="id">Identificador del inventario proveniente de la ruta.</param>
        /// <param name="pInventory">Entidad <see cref="Inventory"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id,
            [Bind("InventoryId,PurchasePrice,SalePrice,MinimumStock,CurrentStock,ProductId,StatusId")]
            Inventory pInventory)
        {
            if (id != pInventory.InventoryId) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pInventory);
            }

            try
            {
                await InventoryBL.ModificarAsync(pInventory);
                TempData["Success"] = "Inventario modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pInventory);
            }
        }

        // GET: Inventories/Delete/5
        /// <summary>
        /// Muestra la confirmación para realizar la eliminación lógica de un inventario.
        /// </summary>
        /// <param name="id">Identificador del inventario a desactivar.</param>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var inventory = await InventoryBL.ObtenerPorIdAsync(new Inventory { InventoryId = id.Value });
                if (inventory == null) return NotFound();
                return View(inventory);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inventories/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del inventario cambiando su estado a "Inactivo" (StatusId = 2).
        /// </summary>
        /// <param name="id">Identificador del inventario a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await InventoryBL.EliminarAsync(new Inventory { InventoryId = id, StatusId = 2 });
                TempData["Success"] = "Inventario desactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ── Métodos Privados ─────────────────────────────────────────────────────

        /// <summary>
        /// Carga las listas de productos activos y estados de inventario
        /// necesarios para los controles desplegables de las vistas Create y Edit.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.ProductList = new SelectList(
                await ProductListDAL.ObtenerTodosAsync(new ProductList { StatusId = 1 }),
                "ProductId", "Name");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 4 }, pIsActive: true),
                "StatusId", "Name");
        }
    }
}