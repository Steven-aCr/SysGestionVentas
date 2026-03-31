using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        /// <summary>
        /// Muestra la lista de todos los usuarios del sistema.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await UsersDAL.ObtenerTodosAsync(new User());
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<User>());
            }
        }

        // GET: Users/Details/5
        /// <summary>
        /// Muestra el detalle de un usuario específico.
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var user = await UsersDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
                if (user == null)
                    return NotFound();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Users/Create
        /// <summary>
        /// Muestra el formulario para crear un nuevo usuario.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        /// <summary>
        /// Procesa la creación de un nuevo usuario.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User pUser)
        {
            if (!ModelState.IsValid)
                return View(pUser);
            try
            {
                await UsersDAL.GuardarAsync(pUser);
                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pUser);
            }
        }

        // GET: Users/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un usuario existente.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var user = await UsersDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
                if (user == null)
                    return NotFound();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/Edit/5
        /// <summary>
        /// Procesa la modificación de un usuario existente.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User pUser)
        {
            if (id != pUser.UserId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(pUser);
            try
            {
                await UsersDAL.ModificarAsync(pUser);
                TempData["Success"] = "Usuario modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pUser);
            }
        }

        // GET: Users/Delete/5
        /// <summary>
        /// Muestra la confirmación para desactivar un usuario (eliminación lógica).
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var user = await UsersDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
                if (user == null)
                    return NotFound();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del usuario cambiando su estado a inactivo.
        /// StatusId = 2 corresponde al estado "Inactivo" según el seed data del script SQL.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // StatusId = 2 → "Inactivo" según el seed data del script SQL
                await UsersDAL.EliminarAsync(new User { UserId = id, StatusId = 2 });
                TempData["Success"] = "Usuario desactivado correctamente.";
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