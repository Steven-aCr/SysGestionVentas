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
    public class ClientsController : Controller
    {
        // GET: Clients
        /// <summary>
        /// Muestra la lista paginada de clientes con soporte de búsqueda y filtro por estado.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto libre de búsqueda sobre nombre o apellido de la persona.</param>
        /// <param name="statusId">Filtro opcional por estado de la persona asociada.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null, int statusId = 0)
        {
            try
            {
                var query = new PagedQuery<Client>
                {
                    Filter = new Client
                    {
                        Person = new Person { StatusId = statusId }
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await ClientBL.BuscarAsync(query);

                // Filtro en memoria sobre nombre/apellido si se proporcionó búsqueda
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    var q = busqueda.ToLower();
                    resultado.Items = resultado.Items
                        .Where(c =>
                            (c.Person?.FirstName?.ToLower().Contains(q) ?? false) ||
                            (c.Person?.LastName?.ToLower().Contains(q) ?? false) ||
                            (c.Person?.Dui?.ToLower().Contains(q) ?? false) ||
                            (c.Person?.PhoneNumber?.ToLower().Contains(q) ?? false))
                        .ToList();
                }

                ViewBag.Busqueda = busqueda;
                ViewBag.StatusId = statusId;
                await CargarFiltrosAsync(statusId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Client>());
            }
        }

        // GET: Clients/Details/5
        /// <summary>
        /// Muestra el detalle de un cliente específico, incluyendo
        /// los datos de su <see cref="Person"/> asociada y su estado.
        /// </summary>
        /// <param name="id">Identificador del cliente a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var client = await ClientBL.ObtenerPorIdAsync(new Client { ClientId = id.Value });
                if (client == null) return NotFound();
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Clients/Delete/5
        /// <summary>
        /// Muestra la confirmación para realizar la eliminación lógica de un cliente.
        /// Desactiva el estado de la <see cref="Person"/> asociada.
        /// </summary>
        /// <param name="id">Identificador del cliente a desactivar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var client = await ClientBL.ObtenerPorIdAsync(new Client { ClientId = id.Value });
                if (client == null) return NotFound();
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Clients/Delete/5
        /// <summary>
        /// Ejecuta la eliminación lógica del cliente cambiando el estado de su
        /// <see cref="Person"/> asociada a "Inactivo" (StatusId = 2).
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="id">Identificador del cliente a desactivar.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var client = new Client
                {
                    ClientId = id,
                    Person = new Person { StatusId = 2 }
                };
                await ClientBL.EliminarAsync(client);
                TempData["Success"] = "Cliente desactivado correctamente.";
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
        /// Carga los filtros desplegables de estado para la vista Index,
        /// manteniendo la selección actual del usuario.
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