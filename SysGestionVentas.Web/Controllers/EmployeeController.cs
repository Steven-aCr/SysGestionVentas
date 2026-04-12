using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeBL _employeeBL;

        // Constructor con inyección de dependencias
        public EmployeeController(EmployeeBL employeeBL)
        {
            _employeeBL = employeeBL ?? throw new ArgumentNullException(nameof(employeeBL));
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _employeeBL.ObtenerTodosAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Employee>());
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var employee = await _employeeBL.ObtenerPorIdAsync(id.Value);
                if (employee == null)
                    return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
                return View(employee);
            try
            {
                await _employeeBL.GuardarAsync(employee);
                TempData["Success"] = "Empleado creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employee);
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var employee = await _employeeBL.ObtenerPorIdAsync(id.Value);
                if (employee == null)
                    return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(employee);
            try
            {
                await _employeeBL.ModificarAsync(employee);
                TempData["Success"] = "Empleado modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employee);
            }
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var employee = await _employeeBL.ObtenerPorIdAsync(id.Value);
                if (employee == null)
                    return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _employeeBL.EliminarAsync(id);
                TempData["Success"] = "Empleado eliminado correctamente.";
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
