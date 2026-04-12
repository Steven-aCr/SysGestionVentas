using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador,Vendedor")]
    public class ProductsController : Controller
    {
        // GET: Products
        /// <summary>
        /// Muestra la lista paginada de productos con soporte de búsqueda y filtros.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto libre de búsqueda sobre nombre o código de barras.</param>
        /// <param name="statusId">Filtro opcional por estado del producto.</param>
        /// <param name="categoryId">Filtro opcional por categoría del producto.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null,
            int statusId = 0, int categoryId = 0)
        {
            try
            {
                var query = new PagedQuery<ProductList>
                {
                    Filter = new ProductList
                    {
                        Name = busqueda,
                        StatusId = statusId,
                        CategoryId = categoryId
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await ProductListBL.BuscarAsync(query);

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                ViewBag.CategoryId = categoryId;
                await CargarFiltrosAsync(statusId, categoryId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<ProductList>());
            }
        }

        // GET: Products/Details/5
        /// <summary>
        /// Muestra el detalle de un producto específico incluyendo su categoría,
        /// inventario y usuario creador.
        /// </summary>
        /// <param name="id">Identificador del producto a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var product = await ProductListBL.ObtenerPorIdAsync(new ProductList { ProductId = id.Value });
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo producto.
        /// Solo accesible por el rol Administrador.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new ProductList());
        }

        // POST: Products/Create
        /// <summary>
        /// Procesa el registro de un nuevo producto.
        /// Asigna automáticamente el usuario autenticado como <c>CreatedByUser</c>.
        /// Verifica unicidad del código de barras en la capa BL/DAL.
        /// </summary>
        /// <param name="pProduct">Entidad <see cref="ProductList"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,Barcode,ImageUrl,CategoryId,StatusId")]
            ProductList pProduct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado.");
                await CargarListasAsync();
                return View(pProduct);
            }

            pProduct.CreatedByUser = userId;

            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pProduct); }

            try
            {
                await ProductListBL.GuardarAsync(pProduct);
                TempData["Success"] = "Producto registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pProduct);
            }
        }

        // GET: Products/Edit/5
        /// <summary>Muestra el formulario para editar un producto existente.</summary>
        /// <param name="id">Identificador del producto a editar.</param>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var product = await ProductListBL.ObtenerPorIdAsync(new ProductList { ProductId = id.Value });
                if (product == null) return NotFound();
                await CargarListasAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Edit/5
        /// <summary>
        /// Procesa la modificación de un producto existente.
        /// No permite cambiar el <c>CreatedByUser</c> ya que es un campo de auditoría.
        /// </summary>
        /// <param name="id">Identificador del producto proveniente de la ruta.</param>
        /// <param name="pProduct">Entidad <see cref="ProductList"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ProductId,Name,Description,Barcode,ImageUrl,CategoryId,StatusId,CreatedByUser")]
            ProductList pProduct)
        {
            if (id != pProduct.ProductId) return NotFound();
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pProduct); }

            try
            {
                await ProductListBL.ModificarAsync(pProduct);
                TempData["Success"] = "Producto modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pProduct);
            }
        }

        // GET: Products/Delete/5
        /// <summary>Muestra la confirmación para la eliminación lógica de un producto.</summary>
        /// <param name="id">Identificador del producto a desactivar.</param>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var product = await ProductListBL.ObtenerPorIdAsync(new ProductList { ProductId = id.Value });
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del producto cambiando su estado a "Inactivo" (StatusId = 2).
        /// </summary>
        /// <param name="id">Identificador del producto a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await ProductListBL.EliminarAsync(new ProductList { ProductId = id, StatusId = 2 });
                TempData["Success"] = "Producto desactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ── Métodos Privados ──────────────────────────────────────────────────────

        /// <summary>Carga las listas de categorías y estados para los formularios Create y Edit.</summary>
        private async Task CargarListasAsync()
        {
            ViewBag.CategoryList = new SelectList(
                await CategoryDAL.ObtenerTodosAsync(new Category { StatusId = 1 }),
                "CategoryId", "Name");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name");
        }

        /// <summary>Carga los filtros desplegables para la vista Index.</summary>
        /// <param name="statusId">ID de estado actualmente filtrado.</param>
        /// <param name="categoryId">ID de categoría actualmente filtrada.</param>
        private async Task CargarFiltrosAsync(int statusId, int categoryId)
        {
            ViewBag.CategoryList = new SelectList(
                await CategoryDAL.ObtenerTodosAsync(new Category()),
                "CategoryId", "Name", categoryId);

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name", statusId);
        }
    }
}