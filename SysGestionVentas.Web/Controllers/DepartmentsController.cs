using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador")]
    public class DepartmentsController : Controller
    {
        // GET: Departments
        /// <summary>
        /// Muestra la lista de departamentos con soporte de búsqueda y filtro por estado.
        /// </summary>
        /// <param name="busqueda">Texto libre de búsqueda sobre el nombre del departamento.</param>
        /// <param name="statusId">Filtro opcional por estado del departamento.</param>
        public async Task<IActionResult> Index(string? busqueda = null, int statusId = 0)
        {
            try
            {
                var resultado = await DepartmentBL.ObtenerTodosAsync(new Department
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
                return View(new List<Department>());
            }
        }

        // GET: Departments/Details/5
        /// <summary>
        /// Muestra el detalle de un departamento específico incluyendo su estado.
        /// </summary>
        /// <param name="id">Identificador del departamento a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dept = await DepartmentBL.ObtenerPorIdAsync(new Department { DepartmentId = id.Value });
                if (dept == null) return NotFound();
                return View(dept);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Departments/Create
        /// <summary>Muestra el formulario para crear un nuevo departamento.</summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Department());
        }

        // POST: Departments/Create
        /// <summary>
        /// Procesa el registro de un nuevo departamento. Valida unicidad del nombre.
        /// </summary>
        /// <param name="pDepartment">Entidad <see cref="Department"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,StatusId")] Department pDepartment)
        {
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pDepartment); }

            try
            {
                await DepartmentBL.GuardarAsync(pDepartment);
                TempData["Success"] = "Departamento registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDepartment);
            }
        }

        // GET: Departments/Edit/5
        /// <summary>Muestra el formulario para editar un departamento existente.</summary>
        /// <param name="id">Identificador del departamento a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dept = await DepartmentBL.ObtenerPorIdAsync(new Department { DepartmentId = id.Value });
                if (dept == null) return NotFound();
                await CargarListasAsync();
                return View(dept);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Departments/Edit/5
        /// <summary>
        /// Procesa la modificación de un departamento existente.
        /// </summary>
        /// <param name="id">Identificador del departamento proveniente de la ruta.</param>
        /// <param name="pDepartment">Entidad <see cref="Department"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DepartmentId,Name,Description,StatusId")] Department pDepartment)
        {
            if (id != pDepartment.DepartmentId) return NotFound();
            if (!ModelState.IsValid) { await CargarListasAsync(); return View(pDepartment); }

            try
            {
                await DepartmentBL.ModificarAsync(pDepartment);
                TempData["Success"] = "Departamento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDepartment);
            }
        }

        // GET: Departments/Delete/5
        /// <summary>Muestra la confirmación para la eliminación lógica de un departamento.</summary>
        /// <param name="id">Identificador del departamento a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dept = await DepartmentBL.ObtenerPorIdAsync(new Department { DepartmentId = id.Value });
                if (dept == null) return NotFound();
                return View(dept);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Departments/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del departamento cambiando su estado a "Inactivo" (StatusId = 2).
        /// </summary>
        /// <param name="id">Identificador del departamento a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await DepartmentBL.EliminarAsync(new Department { DepartmentId = id, StatusId = 2 });
                TempData["Success"] = "Departamento desactivado correctamente.";
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