using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador")]
    public class CategoriesController : Controller
    {
        // GET: Categories
        /// <summary>
        /// Muestra la lista de categorías con soporte de búsqueda y filtro por estado.
        /// </summary>
        /// <param name="busqueda">Texto libre de búsqueda sobre el nombre de la categoría.</param>
        /// <param name="statusId">Filtro opcional por estado de la categoría.</param>
        public async Task<IActionResult> Index(string? busqueda = null, int statusId = 0)
        {
            try
            {
                var resultado = await CategoryBL.ObtenerTodosAsync(new Category
                {
                    Name = busqueda,
                    StatusId = statusId
                });

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                await CargarFiltrosAsync(statusId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Category>());
            }
        }

        // GET: Categories/Details/5
        /// <summary>
        /// Muestra el detalle de una categoría específica incluyendo su estado y usuario creador.
        /// </summary>
        /// <param name="id">Identificador de la categoría a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await CategoryBL.ObtenerPorIdAsync(new Category { CategoryId = id.Value });
                if (category == null) return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Categories/Create
        /// <summary>Muestra el formulario para crear una nueva categoría.</summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Category());
        }

        // POST: Categories/Create
        /// <summary>
        /// Procesa el registro de una nueva categoría.
        /// Asigna automáticamente el usuario autenticado como <c>CreatedByUser</c>.
        /// </summary>
        /// <param name="pCategory">Entidad <see cref="Category"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,StatusId")] Category pCategory)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado.");
                await CargarListasAsync();
                return View(pCategory);
            }

            pCategory.CreatedByUser = userId;

            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pCategory); }

            try
            {
                await CategoryBL.GuardarAsync(pCategory);
                TempData["Success"] = "Categoría registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pCategory);
            }
        }

        // GET: Categories/Edit/5
        /// <summary>Muestra el formulario para editar una categoría existente.</summary>
        /// <param name="id">Identificador de la categoría a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await CategoryBL.ObtenerPorIdAsync(new Category { CategoryId = id.Value });
                if (category == null) return NotFound();
                await CargarListasAsync();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Categories/Edit/5
        /// <summary>
        /// Procesa la modificación de una categoría existente.
        /// </summary>
        /// <param name="id">Identificador de la categoría proveniente de la ruta.</param>
        /// <param name="pCategory">Entidad <see cref="Category"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("CategoryId,Name,Description,StatusId,CreatedByUser")] Category pCategory)
        {
            if (id != pCategory.CategoryId) return NotFound();
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pCategory); }

            try
            {
                await CategoryBL.ModificarAsync(pCategory);
                TempData["Success"] = "Categoría modificada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pCategory);
            }
        }

        // GET: Categories/Delete/5
        /// <summary>Muestra la confirmación para la eliminación lógica de una categoría.</summary>
        /// <param name="id">Identificador de la categoría a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var category = await CategoryBL.ObtenerPorIdAsync(new Category { CategoryId = id.Value });
                if (category == null) return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Categories/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica de la categoría cambiando su estado a "Inactivo" (StatusId = 2).
        /// </summary>
        /// <param name="id">Identificador de la categoría a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await CategoryBL.EliminarAsync(new Category { CategoryId = id, StatusId = 2 });
                TempData["Success"] = "Categoría desactivada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ── Métodos Privados ──────────────────────────────────────────────────────

        /// <summary>Carga las listas de estados para los formularios Create y Edit.</summary>
        private async Task CargarListasAsync()
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name");
        }

        /// <summary>Carga los filtros desplegables para la vista Index.</summary>
        /// <param name="statusId">ID de estado actualmente filtrado.</param>
        private async Task CargarFiltrosAsync(int statusId)
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name", statusId);
        }
    }
}