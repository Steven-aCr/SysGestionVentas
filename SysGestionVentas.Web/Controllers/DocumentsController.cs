using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador,Vendedor")]
    public class DocumentsController : Controller
    {
        // GET: Documents
        /// <summary>
        /// Muestra la lista paginada de documentos con soporte de búsqueda y filtros.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto de búsqueda parcial sobre el número de documento.</param>
        /// <param name="statusId">Filtro opcional por estado del documento.</param>
        /// <param name="docTypeId">Filtro opcional por tipo de documento.</param>
        public async Task<IActionResult> Index(int page = 1, string? busqueda = null,
            int statusId = 0, int docTypeId = 0)
        {
            try
            {
                var query = new PagedQuery<Document>
                {
                    Filter = new Document
                    {
                        DocNumber = busqueda,
                        StatusId = statusId,
                        DocTypeId = docTypeId
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await DocumentBL.BuscarAsync(query);
                ViewBag.Busqueda = busqueda;
                await CargarFiltrosAsync(statusId, docTypeId);
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Document>());
            }
        }

        // GET: Documents/Details/5
        /// <summary>
        /// Muestra el detalle de un documento específico junto con sus líneas de detalle.
        /// </summary>
        /// <param name="id">Identificador del documento a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var document = await DocumentBL.ObtenerPorIdAsync(new Document { DocumentId = id.Value });
                if (document == null) return NotFound();

                var detalles = await DocumentDetailBL.ObtenerPorDocumentoAsync(id.Value);
                ViewBag.Detalles = detalles;
                return View(document);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Obtiene en formato JSON los movimientos de inventario asociados a un documento,
        /// para su consumo desde la vista de detalle vía fetch.
        /// </summary>
        /// <param name="id">Identificador del documento.</param>
        /// <returns>Lista de movimientos serializados o NotFound si el documento no existe.</returns>
        [HttpGet]
        public async Task<IActionResult> Movimientos(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var movimientos = await InventoryMovementBL.ObtenerPorDocumentoAsync(id.Value);

                var resultado = movimientos.Select(m => new
                {
                    m.InventoryMovementId,
                    MovementType = m.MovementType?.Name,
                    Product = m.Inventory?.Product?.Name,
                    m.Quantity,
                    m.UnitCost,
                    CreatedBy = m.CreatedBy?.UserName,
                    m.Notes
                });

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: Documents/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo documento.
        /// El usuario creador se asigna automáticamente desde la sesión.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Document { IssueDate = DateTime.Today });
        }

        // POST: Documents/Create
        /// <summary>
        /// Procesa el registro de un nuevo documento.
        /// Asigna automáticamente el usuario autenticado como <c>CreatedByUser</c>.
        /// </summary>
        /// <param name="pDocument">Entidad <see cref="Document"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DocTypeId,DocNumber,IssueDate,PersonId,StatusId")]
            Document pDocument)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado.");
                await CargarListasAsync();
                return View(pDocument);
            }

            pDocument.CreatedByUser = userId;
            pDocument.TotalAmount = 0;

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pDocument);
            }

            try
            {
                await DocumentBL.GuardarAsync(pDocument);
                TempData["Success"] = "Documento registrado correctamente.";
                return RedirectToAction(nameof(Details), new { id = pDocument.DocumentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDocument);
            }
        }

        // GET: Documents/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un documento existente.
        /// Solo documentos en estado "Emitido" o "Pendiente" pueden editarse.
        /// </summary>
        /// <param name="id">Identificador del documento a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var document = await DocumentBL.ObtenerPorIdAsync(new Document { DocumentId = id.Value });
                if (document == null) return NotFound();

                await CargarListasAsync();
                return View(document);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Documents/Edit/5
        /// <summary>
        /// Procesa la modificación de un documento existente.
        /// No permite cambiar <c>DocNumber</c>, <c>DocTypeId</c> ni <c>CreatedByUser</c>.
        /// </summary>
        /// <param name="id">Identificador del documento proveniente de la ruta.</param>
        /// <param name="pDocument">Entidad <see cref="Document"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DocumentId,DocTypeId,DocNumber,IssueDate,TotalAmount,PersonId,CreatedByUser,StatusId")]
            Document pDocument)
        {
            if (id != pDocument.DocumentId) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pDocument);
            }

            try
            {
                await DocumentBL.ModificarAsync(pDocument);
                TempData["Success"] = "Documento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pDocument);
            }
        }

        // GET: Documents/Delete/5
        /// <summary>
        /// Muestra la confirmación para anular un documento (eliminación lógica).
        /// </summary>
        /// <param name="id">Identificador del documento a anular.</param>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var document = await DocumentBL.ObtenerPorIdAsync(new Document { DocumentId = id.Value });
                if (document == null) return NotFound();
                return View(document);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Documents/Delete/5
        /// <summary>
        /// Ejecuta la anulación lógica del documento cambiando su estado a "Anulado" (StatusId = 5).
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="id">Identificador del documento a anular.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // StatusId = 5 corresponde al estado "Anulado" según el seed data del script SQL.
                await DocumentBL.EliminarAsync(new Document { DocumentId = id, StatusId = 5 });
                TempData["Success"] = "Documento anulado correctamente.";
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
        /// Carga las listas de tipos de documento, personas y estados
        /// necesarias para los controles desplegables de las vistas Create y Edit.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.DocTypeList = new SelectList(
                await DocumentTypeDAL.ObtenerTodosAsync(new DocumentType()),
                "DocTypeId", "Name");

            ViewBag.PersonList = new SelectList(
                await PersonDAL.ObtenerTodosAsync(new Person { StatusId = 1 }),
                "PersonId", "FullName");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 3 }, pIsActive: true),
                "StatusId", "Name");
        }

        /// <summary>
        /// Carga los filtros desplegables para la vista Index manteniendo las selecciones actuales.
        /// </summary>
        /// <param name="statusId">ID de estado actualmente filtrado.</param>
        /// <param name="docTypeId">ID de tipo de documento actualmente filtrado.</param>
        private async Task CargarFiltrosAsync(int statusId, int docTypeId)
        {
            ViewBag.DocTypeList = new SelectList(
                await DocumentTypeDAL.ObtenerTodosAsync(new DocumentType()),
                "DocTypeId", "Name", docTypeId);

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 3 }, pIsActive: true),
                "StatusId", "Name", statusId);
        }
    }
}