using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.ViewModels;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador")]
    public class EmployeesController : Controller
    {
        // GET: Employees
        /// <summary>
        /// Muestra la lista paginada de empleados con soporte de búsqueda y filtros.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto libre de búsqueda sobre código de empleado o nombre.</param>
        /// <param name="statusId">Filtro opcional por estado del empleado.</param>
        /// <param name="departmentId">Filtro opcional por departamento.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null,
            int statusId = 0, int departmentId = 0)
        {
            try
            {
                var query = new PagedQuery<Employee>
                {
                    Filter = new Employee
                    {
                        EmployeeCode = busqueda,
                        StatusId = statusId,
                        DepartmentId = departmentId > 0 ? departmentId : null
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await EmployeeBL.BuscarAsync(query);

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                ViewBag.DepartmentId = departmentId;
                await CargarFiltrosAsync(statusId, departmentId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Employee>());
            }
        }

        // GET: Employees/Details/5
        /// <summary>
        /// Muestra el detalle de un empleado específico, incluyendo sus relaciones
        /// con <see cref="Person"/>, <see cref="Department"/>, <see cref="User"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="id">Identificador del empleado a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = await EmployeeBL.ObtenerPorIdAsync(new Employee { EmployeeId = id.Value });
                if (employee == null) return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employees/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo empleado en el sistema.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();

            var model = new CreateEmployeeModel
            {
                HireDate = DateTime.Today
            };

            return View(model);
        }

        // POST: Employees/Create
        /// <summary>
        /// Procesa el registro de un nuevo empleado.
        /// Aplica validaciones de estructura y de negocio antes de persistir.
        /// </summary>
        /// <param name="pEmployee">Entidad <see cref="Employee"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(model);
            }

            try
            {
                await EmployeeBL.CrearConPersonaAsync(model); 

                TempData["Success"] = "Empleado registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(model); 
            }
        }

        // GET: Employees/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un empleado existente.
        /// </summary>
        /// <param name="id">Identificador del empleado a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = await EmployeeBL.ObtenerPorIdAsync(new Employee { EmployeeId = id.Value });
                if (employee == null) return NotFound();

                await CargarListasAsync();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employees/Edit/5
        /// <summary>
        /// Procesa la modificación de un empleado existente.
        /// </summary>
        /// <param name="id">Identificador del empleado proveniente de la ruta.</param>
        /// <param name="pEmployee">Entidad <see cref="Employee"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("EmployeeId,EmployeeCode,HireDate,Salary,DepartmentId,UserId,PersonId,StatusId")]
            Employee pEmployee)
        {
            if (id != pEmployee.EmployeeId) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pEmployee);
            }

            try
            {
                await EmployeeBL.ModificarAsync(pEmployee);
                TempData["Success"] = "Empleado modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pEmployee);
            }
        }

        // GET: Employees/Delete/5
        /// <summary>
        /// Muestra la confirmación para realizar la eliminación lógica de un empleado.
        /// </summary>
        /// <param name="id">Identificador del empleado a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = await EmployeeBL.ObtenerPorIdAsync(new Employee { EmployeeId = id.Value });
                if (employee == null) return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employees/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del empleado cambiando su estado a "Inactivo" (StatusId = 2).
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="id">Identificador del empleado a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await EmployeeBL.EliminarAsync(new Employee { EmployeeId = id, StatusId = 2 });
                TempData["Success"] = "Empleado desactivado correctamente.";
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
        /// Carga las listas de personas, departamentos, usuarios y estados necesarias
        /// para los controles desplegables de las vistas Create y Edit.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.PersonList = new SelectList(
                await PersonDAL.ObtenerTodosAsync(new Person { StatusId = 1 }),
                "PersonId", "FullName");

            ViewBag.DepartmentList = new SelectList(
                await DepartmentDAL.ObtenerTodosAsync(new Department { StatusId = 1 }),
                "DepartmentId", "Name");

            ViewBag.UserList = new SelectList(
                await UserDAL.ObtenerTodosAsync(new User { StatusId = 1 }),
                "UserId", "UserName");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name");
        }

        /// <summary>
        /// Carga los filtros desplegables para la vista Index manteniendo las selecciones actuales.
        /// </summary>
        /// <param name="statusId">ID de estado actualmente filtrado.</param>
        /// <param name="departmentId">ID de departamento actualmente filtrado.</param>
        private async Task CargarFiltrosAsync(int statusId, int departmentId)
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1 }, pIsActive: true),
                "StatusId", "Name", statusId);

            ViewBag.DepartmentFilterList = new SelectList(
                await DepartmentDAL.ObtenerTodosAsync(new Department()),
                "DepartmentId", "Name", departmentId);
        }
    }
}