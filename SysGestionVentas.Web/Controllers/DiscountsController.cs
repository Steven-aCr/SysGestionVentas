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
    [Authorize(Roles = "Administrador")]
    public class DiscountsController : Controller
    {
        // GET: Discounts
        /// <summary>
        /// Muestra la lista paginada de descuentos con soporte de búsqueda, filtro por estado
        /// y rango de fechas de vigencia.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto libre de búsqueda sobre el nombre del descuento.</param>
        /// <param name="statusId">Filtro opcional por estado del descuento.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null, int statusId = 0)
        {
            try
            {
                var query = new PagedQuery<Discount>
                {
                    Filter = new Discount
                    {
                        Name = busqueda,
                        StatusId = statusId
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await DiscountBL.BuscarAsync(query);

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                await CargarFiltrosAsync(statusId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Discount>());
            }
        }

        // GET: Discounts/Details/5
        /// <summary>
        /// Muestra el detalle de un descuento específico incluyendo su porcentaje,
        /// rango de fechas y estado.
        /// </summary>
        /// <param name="id">Identificador del descuento a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var discount = await DiscountBL.ObtenerPorIdAsync(new Discount { DiscountId = id.Value });
                if (discount == null) return NotFound();
                return View(discount);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Discounts/Create
        /// <summary>Muestra el formulario para registrar un nuevo descuento.</summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Discount
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            });
        }

        // POST: Discounts/Create
        /// <summary>
        /// Procesa el registro de un nuevo descuento.
        /// Verifica unicidad del nombre y coherencia del rango de fechas.
        /// </summary>
        /// <param name="pDiscount">Entidad <see cref="Discount"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Percentage,StartDate,EndDate,StatusId")] Discount pDiscount)
        {
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pDiscount); }

            try
            {
                await DiscountBL.GuardarAsync(pDiscount);
                TempData["Success"] = "Descuento registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDiscount);
            }
        }

        // GET: Discounts/Edit/5
        /// <summary>Muestra el formulario para editar un descuento existente.</summary>
        /// <param name="id">Identificador del descuento a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var discount = await DiscountBL.ObtenerPorIdAsync(new Discount { DiscountId = id.Value });
                if (discount == null) return NotFound();
                await CargarListasAsync();
                return View(discount);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Discounts/Edit/5
        /// <summary>
        /// Procesa la modificación de un descuento existente.
        /// Verifica unicidad del nombre y coherencia del rango de fechas.
        /// </summary>
        /// <param name="id">Identificador del descuento proveniente de la ruta.</param>
        /// <param name="pDiscount">Entidad <see cref="Discount"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DiscountId,Name,Percentage,StartDate,EndDate,StatusId")] Discount pDiscount)
        {
            if (id != pDiscount.DiscountId) return NotFound();
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pDiscount); }

            try
            {
                await DiscountBL.ModificarAsync(pDiscount);
                TempData["Success"] = "Descuento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDiscount);
            }
        }

        // GET: Discounts/Delete/5
        /// <summary>Muestra la confirmación para la eliminación lógica de un descuento.</summary>
        /// <param name="id">Identificador del descuento a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var discount = await DiscountBL.ObtenerPorIdAsync(new Discount { DiscountId = id.Value });
                if (discount == null) return NotFound();
                return View(discount);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Discounts/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del descuento cambiando su estado a "Inactivo" (StatusId = 2).
        /// </summary>
        /// <param name="id">Identificador del descuento a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await DiscountBL.EliminarAsync(new Discount { DiscountId = id, StatusId = 2 });
                TempData["Success"] = "Descuento desactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ── Métodos Privados ──────────────────────────────────────────────────────

        /// <summary>Carga la lista de estados para los formularios Create y Edit.</summary>
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