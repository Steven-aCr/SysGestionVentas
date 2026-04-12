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
    public class SuppliersController : Controller
    {
        // GET: Suppliers
        /// <summary>
        /// Muestra la lista paginada de proveedores con soporte de búsqueda y filtros.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto libre de búsqueda sobre nombre de empresa o NIT.</param>
        /// <param name="statusId">Filtro opcional por estado del proveedor.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null, int statusId = 0)
        {
            try
            {
                var query = new PagedQuery<Supplier>
                {
                    Filter = new Supplier
                    {
                        CompanyName = busqueda,
                        StatusId = statusId
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await SupplierBL.BuscarAsync(query);

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                await CargarFiltrosAsync(statusId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Supplier>());
            }
        }

        // GET: Suppliers/Details/5
        /// <summary>
        /// Muestra el detalle de un proveedor específico, incluyendo sus relaciones
        /// con <see cref="Person"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="id">Identificador del proveedor a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var supplier = await SupplierBL.ObtenerPorIdAsync(new Supplier { SupplierId = id.Value });
                if (supplier == null) return NotFound();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Suppliers/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo proveedor en el sistema.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Supplier());
        }

        // POST: Suppliers/Create
        /// <summary>
        /// Procesa el registro de un nuevo proveedor.
        /// Verifica unicidad de NIT y NRC en la capa BL/DAL.
        /// </summary>
        /// <param name="pSupplier">Entidad <see cref="Supplier"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PersonId,CompanyName,Nit,Nrc,Description,StatusId")]
            Supplier pSupplier)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pSupplier);
            }

            try
            {
                await SupplierBL.GuardarAsync(pSupplier);
                TempData["Success"] = "Proveedor registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pSupplier);
            }
        }

        // GET: Suppliers/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un proveedor existente.
        /// </summary>
        /// <param name="id">Identificador del proveedor a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var supplier = await SupplierBL.ObtenerPorIdAsync(new Supplier { SupplierId = id.Value });
                if (supplier == null) return NotFound();

                await CargarListasAsync();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Suppliers/Edit/5
        /// <summary>
        /// Procesa la modificación de un proveedor existente.
        /// Verifica unicidad de NIT y NRC antes de actualizar.
        /// </summary>
        /// <param name="id">Identificador del proveedor proveniente de la ruta.</param>
        /// <param name="pSupplier">Entidad <see cref="Supplier"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("SupplierId,PersonId,CompanyName,Nit,Nrc,Description,StatusId")]
            Supplier pSupplier)
        {
            if (id != pSupplier.SupplierId) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pSupplier);
            }

            try
            {
                await SupplierBL.ModificarAsync(pSupplier);
                TempData["Success"] = "Proveedor modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pSupplier);
            }
        }

        // GET: Suppliers/Delete/5
        /// <summary>
        /// Muestra la confirmación para realizar la eliminación lógica de un proveedor.
        /// </summary>
        /// <param name="id">Identificador del proveedor a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var supplier = await SupplierBL.ObtenerPorIdAsync(new Supplier { SupplierId = id.Value });
                if (supplier == null) return NotFound();
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Suppliers/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del proveedor cambiando su estado a "Inactivo" (StatusId = 2).
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="id">Identificador del proveedor a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await SupplierBL.EliminarAsync(new Supplier { SupplierId = id, StatusId = 2 });
                TempData["Success"] = "Proveedor desactivado correctamente.";
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
        /// Carga las listas de personas activas y estados necesarios para los
        /// controles desplegables de las vistas Create y Edit.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.PersonList = new SelectList(
                await PersonDAL.ObtenerTodosAsync(new Person { StatusId = 1 }),
                "PersonId", "FullName");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name");
        }

        /// <summary>
        /// Carga los filtros desplegables para la vista Index manteniendo la selección actual.
        /// </summary>
        /// <param name="statusId">ID de estado actualmente filtrado.</param>
        private async Task CargarFiltrosAsync(int statusId)
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name", statusId);
        }
    }
}