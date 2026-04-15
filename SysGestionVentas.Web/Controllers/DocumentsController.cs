using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using SysGestionVentas.EN.ViewModels;
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

        // GET: Documents/Movimientos/5
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
        /// Muestra el formulario unificado para registrar un nuevo documento.
        /// El formulario incluye la sección de datos del nuevo cliente
        /// (que se registrará como <see cref="Person"/> y <see cref="Client"/>
        /// en la misma transacción).
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new CreateDocumentModel { IssueDate = DateTime.Today });
        }

        // POST: Documents/Create
        /// <summary>
        /// Confirma y persiste en una única transacción atómica:
        /// <list type="number">
        ///   <item>La <see cref="Person"/> y el <see cref="Client"/> del nuevo cliente.</item>
        ///   <item>El encabezado del <see cref="Document"/>.</item>
        ///   <item>Las líneas de <see cref="DocumentDetail"/> con cálculo de montos.</item>
        ///   <item>Los <see cref="InventoryMovement"/> con actualización de stock.</item>
        ///   <item>El total acumulado del documento.</item>
        /// </list>
        /// Requiere al menos una línea de detalle válida para proceder.
        /// Los campos de datos del cliente (<c>FirstName</c>, <c>LastName</c>, etc.)
        /// son obligatorios y se validan en la capa BL/DAL.
        /// </summary>
        /// <param name="pModel">ViewModel con datos del cliente, encabezado y colección de detalles.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentModel pModel)
        {
            // 1 — Asignar usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    "No se pudo identificar al usuario autenticado.");
                await CargarListasAsync();
                return View(pModel);
            }

            pModel.CreatedByUser = userId;

            // 2 — Validar que exista al menos un detalle
            if (pModel.Detalles == null || pModel.Detalles.Count == 0)
            {
                ModelState.AddModelError(string.Empty,
                    "Debe agregar al menos un producto antes de confirmar.");
                await CargarListasAsync();
                return View(pModel);
            }

            // 3 — Ignorar errores de ModelState para los detalles (vienen de inputs hidden
            //     y sus valores ya están validados en el JS antes del submit).
            foreach (var key in ModelState.Keys
                .Where(k => k.StartsWith("Detalles["))
                .ToList())
            {
                ModelState.Remove(key);
            }

            // 4 — PersonId no viene del formulario: remover de la validación de modelo
            ModelState.Remove(nameof(CreateDocumentModel.PersonId));

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pModel);
            }

            try
            {
                var document = await DocumentBL.CrearConDetallesAsync(pModel);
                TempData["Success"] =
                    $"Documento {document.DocNumber} registrado correctamente.";
                return RedirectToAction(nameof(Details), new { id = document.DocumentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pModel);
            }
        }

        // GET: Documents/GetProductInfo?productId=5
        /// <summary>
        /// Endpoint AJAX que devuelve el precio de venta y el stock disponible
        /// de un producto para autocompletar la línea de detalle en el formulario dinámico.
        /// </summary>
        /// <param name="productId">Identificador del producto a consultar.</param>
        /// <returns>
        /// JSON con <c>success</c>, <c>salePrice</c> y <c>currentStock</c> si el producto
        /// tiene inventario activo registrado; o <c>success: false</c> con un <c>message</c>
        /// descriptivo en caso contrario.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetProductInfo(int productId)
        {
            if (productId <= 0)
                return Json(new { success = false, message = "ID de producto no válido." });

            try
            {
                var inventories = await InventoryDAL.ObtenerTodosAsync(
                    new Inventory { ProductId = productId, StatusId = 1 });

                var inv = inventories.FirstOrDefault();

                if (inv == null)
                    return Json(new
                    {
                        success = false,
                        message = "Este producto no tiene inventario activo registrado."
                    });

                return Json(new
                {
                    success = true,
                    salePrice = inv.SalePrice,
                    currentStock = inv.CurrentStock
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Documents/Edit/5
        /// <summary>
        /// Muestra el formulario para editar los datos de cabecera de un documento existente.
        /// No permite modificar las líneas de detalle desde esta acción.
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
        /// Procesa la modificación de la cabecera de un documento existente.
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
        /// Solo accesible por el rol Administrador.
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
        /// Carga todas las listas desplegables necesarias para las vistas Create y Edit:
        /// tipos de documento, estados y productos con inventario activo.
        /// La lista de personas ya no es necesaria en Create porque el cliente
        /// se crea directamente desde el formulario.
        /// </summary>
        private async Task CargarListasAsync()
        {
            ViewBag.DocTypeList = new SelectList(
                await DocumentTypeDAL.ObtenerTodosAsync(new DocumentType()),
                "DocTypeId", "Name");

            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 3 }, pIsActive: true),
                "StatusId", "Name");

            // Lista de productos para el selector del modal.
            // Se filtra por inventarios activos (StatusId = 1) para evitar mostrar
            // productos sin stock o desactivados.
            var inventarios = await InventoryDAL.ObtenerTodosAsync(
                new Inventory { StatusId = 1 });

            ViewBag.ProductList = inventarios
                .Where(i => i.Product != null)
                .Select(i => new SelectListItem
                {
                    Value = i.ProductId.ToString(),
                    Text = i.Product!.Name
                })
                .ToList();
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