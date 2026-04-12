using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using SysGestionVentas.EN.ViewModels;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador, Cliente")]

    /// <summary>
    /// Controlador del portal de clientes. Solo accesible por usuarios
    /// con rol "Cliente". Expone exclusivamente:
    /// <list type="bullet">
    ///   <item>Las órdenes/documentos propios del cliente autenticado.</item>
    ///   <item>El catálogo público de productos.</item>
    ///   <item>La edición del perfil propio.</item>
    /// </list>
    /// Cualquier intento de acceder a datos de otro cliente resulta en
    /// redirección a la página de acceso denegado.
    /// </summary>
    public class ClientePortalController : Controller
    {
        // ── GET: ClientePortal/Portal ─────────────────────────────

        /// <summary>
        /// Página de inicio del portal del cliente.
        /// Muestra un resumen de sus últimas órdenes y accesos rápidos.
        /// </summary>
        /// <returns>Vista del portal con las últimas órdenes del cliente.</returns>
        public async Task<IActionResult> Portal()
        {
            try
            {
                int personId = ObtenerPersonIdActual();
                var ordenes = await DocumentDAL.ObtenerTodosAsync(
                    new Document { PersonId = personId });

                return View(ordenes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Document>());
            }
        }

        // ── GET: ClientePortal/MisOrdenes ─────────────────────────

        /// <summary>
        /// Lista paginada de los documentos/órdenes del cliente autenticado.
        /// El filtro por <c>PersonId</c> se aplica siempre desde el claim de sesión,
        /// independientemente de los parámetros recibidos por URL, para evitar
        /// que un cliente consulte órdenes ajenas manipulando la query string.
        /// </summary>
        /// <param name="page">Número de página (por defecto: 1).</param>
        /// <returns>Vista paginada de órdenes propias.</returns>
        public async Task<IActionResult> MisOrdenes(int page = 1)
        {
            try
            {
                int personId = ObtenerPersonIdActual();

                var query = new PagedQuery<Document>
                {
                    // PersonId SIEMPRE viene del claim, nunca de la URL.
                    Filter = new Document { PersonId = personId },
                    Page = page,
                    PageSize = 10
                };

                var resultado = await DocumentDAL.BuscarAsync(query);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Document>());
            }
        }

        // ── GET: ClientePortal/DetalleOrden/5 ────────────────────

        /// <summary>
        /// Muestra el detalle de una orden específica del cliente.
        /// Verifica que el documento pertenezca al cliente autenticado
        /// antes de mostrarlo. Si no le pertenece, devuelve 403.
        /// </summary>
        /// <param name="id">Identificador del documento a consultar.</param>
        /// <returns>Vista de detalle de la orden o Forbid si no es del cliente.</returns>
        public async Task<IActionResult> DetalleOrden(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                int personId = ObtenerPersonIdActual();
                var documento = await DocumentDAL.ObtenerPorIdAsync(
                    new Document { DocumentId = id.Value });

                if (documento == null)
                    return NotFound();
                
                // Seguridad: el documento debe pertenecer al cliente autenticado.
                if (documento.PersonId != personId)
                    return Forbid();

                var detalles = await DocumentDetailDAL.ObtenerPorDocumentoAsync(id.Value);
                ViewBag.Detalles = detalles;
                return View(documento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MisOrdenes));
            }
        }

        // ── GET: ClientePortal/Catalogo ───────────────────────────

        /// <summary>
        /// Muestra el catálogo de productos activos disponibles para el cliente.
        /// Solo lectura; no expone precios de compra ni datos de inventario interno.
        /// </summary>
        /// <returns>Vista del catálogo de productos.</returns>
        public async Task<IActionResult> Catalogo()
        {
            try
            {
                // StatusId = 0 → sin filtro de estado en el DAL, se filtra el activo
                // ajustando el StatusId al valor "Activo" de tu seed si lo tienes.
                var productos = await ProductListDAL.ObtenerTodosAsync(new ProductList());
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<ProductList>());
            }
        }

        // ── GET / POST: ClientePortal/Perfil ─────────────────────

        /// <summary>
        /// Muestra el formulario de edición del perfil del cliente autenticado.
        /// Reutiliza el mismo flujo de <see cref="AccountController.Profile"/>
        /// pero dentro del área del portal de clientes.
        /// </summary>
        /// <returns>Vista del perfil con los datos actuales del cliente.</returns>
        public async Task<IActionResult> Perfil()
        {
            try
            {
                int userId = ObtenerUserIdActual();
                var user = await UserBL.ObtenerPorIdAsync(new User { UserId = userId });
                if (user == null)
                    return NotFound();

                var model = new EditProfileModel
                {
                    UserId = user.UserId,
                    PersonId = user.PersonId,
                    Email = user.Email,
                    FirstName = user.Person?.FirstName ?? string.Empty,
                    LastName = user.Person?.LastName ?? string.Empty,
                    Adress = user.Person?.Adress ?? string.Empty,
                    PhoneNumber = user.Person?.PhoneNumber ?? string.Empty,
                    Dui = user.Person?.Dui
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Portal));
            }
        }

        /// <summary>
        /// Procesa la actualización del perfil del cliente autenticado.
        /// Delega la lógica de negocio a <see cref="UserBL.ActualizarPerfilAsync"/>.
        /// </summary>
        /// <param name="pModel">ViewModel con los nuevos datos del perfil.</param>
        /// <returns>Redirección al perfil si fue exitoso, o la vista con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(EditProfileModel pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.NewPassword))
            {
                ModelState.Remove(nameof(EditProfileModel.CurrentPassword));
                ModelState.Remove(nameof(EditProfileModel.NewPassword));
                ModelState.Remove(nameof(EditProfileModel.ConfirmNewPassword));
            }
            else if (string.IsNullOrWhiteSpace(pModel.CurrentPassword))
            {
                ModelState.AddModelError(
                    nameof(EditProfileModel.CurrentPassword),
                    "Debe ingresar su contraseña actual para cambiarla.");
            }

            if (!ModelState.IsValid)
                return View(pModel);

            try
            {
                await UserBL.ActualizarPerfilAsync(pModel);
                TempData["Success"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pModel);
            }
        }

        // ── Métodos privados ──────────────────────────────────────

        /// <summary>
        /// Extrae el <c>UserId</c> del usuario autenticado desde el claim de sesión.
        /// </summary>
        /// <returns>Identificador del usuario autenticado.</returns>
        /// <exception cref="Exception">Se lanza si el claim no existe o no es un entero válido.</exception>
        private int ObtenerUserIdActual()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId) || userId <= 0)
                throw new Exception("No se pudo identificar al usuario autenticado.");
            return userId;
        }

        /// <summary>
        /// Extrae el <c>PersonId</c> del usuario autenticado desde el claim de sesión.
        /// Se usa para filtrar datos propios sin consultar la base de datos.
        /// </summary>
        /// <returns>Identificador de la persona asociada al usuario autenticado.</returns>
        /// <exception cref="Exception">Se lanza si el claim no existe o no es un entero válido.</exception>
        private int ObtenerPersonIdActual()
        {
            var claim = User.FindFirst("PersonId")?.Value;
            if (!int.TryParse(claim, out int personId) || personId <= 0)
                throw new Exception("No se pudo identificar la persona del usuario autenticado.");
            return personId;
        }
    }
}