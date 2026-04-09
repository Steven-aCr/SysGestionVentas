using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentBL _departmentBL;

        // Constructor con inyección de dependencias
        public DepartmentController(DepartmentBL departmentBL)
        {
            _departmentBL = departmentBL ?? throw new ArgumentNullException(nameof(departmentBL));
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            try
            {
                var departments = await _departmentBL.ObtenerTodosAsync();
                return View(departments);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Department>());
            }
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var department = await _departmentBL.ObtenerPorIdAsync(id.Value);
                if (department == null)
                    return NotFound();
                return View(department);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (!ModelState.IsValid)
                return View(department);
            try
            {
                await _departmentBL.GuardarAsync(department);
                TempData["Success"] = "Departamento creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(department);
            }
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var department = await _departmentBL.ObtenerPorIdAsync(id.Value);
                if (department == null)
                    return NotFound();
                return View(department);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(department);
            try
            {
                await _departmentBL.ModificarAsync(department);
                TempData["Success"] = "Departamento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(department);
            }
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var department = await _departmentBL.ObtenerPorIdAsync(id.Value);
                if (department == null)
                    return NotFound();
                return View(department);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departmentBL.EliminarAsync(id);
                TempData["Success"] = "Departamento eliminado correctamente.";
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
