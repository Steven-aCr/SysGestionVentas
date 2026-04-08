using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.ViewModels;

namespace SysGestionVentas.Web.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        /// <summary>
        /// Muestra la lista de usuarios filtrada por pestaña activa y término de búsqueda.
        /// Calcula los conteos por categoría para las insignias de las pestañas.
        /// </summary>
        /// <param name="tab">Pestaña activa: "all", "Vendedor", "Cliente", "inactive".</param>
        /// <param name="search">Término de búsqueda parcial por nombre de usuario.</param>
        public async Task<IActionResult> Index(string tab = "all", string search = "")
        {
            try
            {
                var todos = await UserDAL.ObtenerTodosAsync(new User());

                // Aplicar búsqueda sobre la colección completa
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var q = search.ToLower();
                    todos = todos.Where(u =>
                        (u.UserName?.ToLower().Contains(q) ?? false) ||
                        (u.Email?.ToLower().Contains(q) ?? false) ||
                        (u.Person?.FirstName?.ToLower().Contains(q) ?? false) ||
                        (u.Person?.LastName?.ToLower().Contains(q) ?? false) ||
                        (u.Rol?.Name?.ToLower().Contains(q) ?? false)
                    ).ToList();
                }

                // Conteos para las insignias de pestañas (sobre la lista ya filtrada por búsqueda)
                ViewData["CountAll"] = todos.Count;
                ViewData["CountActive"] = todos.Count(u => u.Status?.Name == "Activo");
                ViewData["CountInactive"] = todos.Count(u => u.Status?.Name != "Activo");
                ViewData["CountVendedor"] = todos.Count(u =>
                    u.Rol?.Name is "Administrador" or "Vendedor" or "Gerente");
                ViewData["CountCliente"] = todos.Count(u => u.Rol?.Name == "Cliente");

                // Filtrar por pestaña activa
                var resultado = tab switch
                {
                    "Vendedor" => todos.Where(u =>
                        u.Rol?.Name is "Administrador" or "Vendedor" or "Gerente").ToList(),
                    "Cliente" => todos.Where(u => u.Rol?.Name == "Cliente").ToList(),
                    "inactive" => todos.Where(u => u.Status?.Name != "Activo").ToList(),
                    _ => todos
                };

                ViewData["ActiveTab"] = tab;
                ViewData["Search"] = search;

                return View(resultado);
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
                var user = await UserDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
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
        /// Muestra el formulario unificado para crear una nueva persona y usuario en una sola operación.
        /// Carga las listas de roles y estados activos necesarios para los combos del formulario.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new CreateUserModel());
        }

        // POST: Users/Create
        /// <summary>
        /// Procesa la creación atómica de una <see cref="Person"/> y su <see cref="User"/> asociado.
        /// Si el modelo no es válido o la transacción falla, regresa el formulario con los errores.
        /// </summary>
        /// <param name="pModel">ViewModel con los datos capturados desde el formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserModel pModel)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pModel);
            }

            try
            {
                await UserBL.CrearConPersonaAsync(pModel);
                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await CargarListasAsync();
                return View(pModel);
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
                var user = await UserDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
                if (user == null)
                    return NotFound();

                await CargarListasAsync();
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
        /// Procesa la modificación de los datos de acceso de un usuario existente.
        /// La contraseña no es modificable desde esta acción.
        /// </summary>
        /// <param name="id">Identificador del usuario proveniente de la ruta.</param>
        /// <param name="pUser">Entidad <see cref="User"/> con los nuevos valores del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User pUser)
        {
            if (id != pUser.UserId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pUser);
            }

            try
            {
                await UserDAL.ModificarAsync(pUser);
                TempData["Success"] = "Usuario modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
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
                var user = await UserDAL.ObtenerPorIdAsync(new User { UserId = id.Value });
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
                await UserDAL.EliminarAsync(new User { UserId = id, StatusId = 2 });
                TempData["Success"] = "Usuario desactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Users/CheckUnique
        /// <summary>
        /// Verifica en tiempo real si un valor de campo único ya existe en el sistema.
        /// Es consumido por las vistas Create y Edit vía fetch (AJAX).
        /// </summary>
        /// <param name="field">
        /// Nombre del campo a verificar. Valores admitidos:
        /// <c>UserName</c>, <c>Email</c>, <c>PhoneNumber</c>, <c>Dui</c>.
        /// </param>
        /// <param name="value">Valor a comprobar.</param>
        /// <param name="excludeId">
        /// Identificador del registro a excluir de la verificación.
        /// Debe enviarse desde la vista Edit para no marcar el propio registro como duplicado.
        /// En la vista Create siempre será 0.
        /// </param>
        /// <returns>
        /// JSON <c>{ available: true }</c> si el valor está libre,
        /// <c>{ available: false }</c> si ya está registrado.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> CheckUnique(string field, string value, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
                return Json(new { available = true });

            bool taken = false;

            try
            {
                switch (field)
                {
                    case "UserName":
                        var byUserName = await UserDAL.ObtenerTodosAsync(
                            new User { UserName = value.Trim() });
                        taken = byUserName.Any(u => u.UserId != excludeId);
                        break;

                    case "Email":
                        taken = await UserDAL.ExisteEmail(
                            new User { Email = value.Trim(), UserId = excludeId }, new DbContexto());
                        break;

                    case "PhoneNumber":
                        var byPhone = await PersonDAL.ObtenerTodosAsync(
                            new Person { PhoneNumber = value.Trim() });
                        taken = byPhone.Any(p => p.PersonId != excludeId);
                        break;

                    case "Dui":
                        var byDui = await PersonDAL.ObtenerTodosAsync(
                            new Person { Dui = value.Trim() });
                        taken = byDui.Any(p => p.PersonId != excludeId);
                        break;

                    default:
                        return Json(new { available = true });
                }
            }
            catch
            {
                // Ante cualquier error de acceso a datos, no bloqueamos la UI
                return Json(new { available = true });
            }

            return Json(new { available = !taken });
        }

        // ── Métodos Privados ─────────────────────────────────────────────────────

        /// <summary>
        /// Carga las listas de roles activos y estados necesarios para los
        /// controles desplegables de las vistas Create y Edit.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.Roles = new SelectList(
                await RolDAL.ObtenerTodosAsync(new Rol { StatusId = 1 }), "RolId", "Name");

            ViewBag.Statuses = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1, 2 }, pIsActive: true),
                "StatusId", "Name");
        }
    }
}