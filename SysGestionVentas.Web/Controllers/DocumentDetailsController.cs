using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador,Vendedor")]
    public class DocumentDetailsController : Controller
    {
        // GET: DocumentDetails/ByDocument/5
        /// <summary>
        /// Muestra todas las líneas de detalle de un documento específico.
        /// </summary>
        /// <param name="documentId">Identificador del documento padre.</param>
        public async Task<IActionResult> ByDocument(int? documentId)
        {
            if (documentId == null) return NotFound();

            try
            {
                var document = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = documentId.Value });

                if (document == null) return NotFound();

                var detalles = await DocumentDetailBL.ObtenerPorDocumentoAsync(documentId.Value);
                ViewBag.Document = document;
                return View(detalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Documents");
            }
        }

        // GET: DocumentDetails/Details/5
        /// <summary>
        /// Muestra el detalle de una línea de documento específica.
        /// </summary>
        /// <param name="id">Identificador del detalle a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var detail = await DocumentDetailBL.ObtenerPorIdAsync(
                    new DocumentDetail { DocDetailId = id.Value });

                if (detail == null) return NotFound();
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Documents");
            }
        }

        // GET: DocumentDetails/Create?documentId=5
        /// <summary>
        /// Muestra el formulario para agregar una línea de detalle a un documento.
        /// Solo disponible si el documento está en estado editable.
        /// </summary>
        /// <param name="documentId">Identificador del documento al que se añadirá el detalle.</param>
        public async Task<IActionResult> Create(int? documentId)
        {
            if (documentId == null) return NotFound();

            try
            {
                var document = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = documentId.Value });

                if (document == null) return NotFound();

                ViewBag.Document = document;
                await CargarListasAsync(documentId.Value);

                return View(new DocumentDetail
                {
                    DocumentId = documentId.Value,
                    TaxPercentage = 13
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Documents");
            }
        }

        // POST: DocumentDetails/Create
        /// <summary>
        /// Procesa el registro de una nueva línea de detalle en el documento indicado.
        /// Calcula automáticamente subtotal, impuesto y total de línea mediante la capa BL.
        /// Genera además el movimiento de inventario correspondiente dentro de una transacción.
        /// </summary>
        /// <param name="pDetail">Entidad <see cref="DocumentDetail"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DocumentId,ProductId,Quantity,UnitPrice,DiscountAmount,TaxPercentage,Notes")]
            DocumentDetail pDetail)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    "No se pudo identificar al usuario autenticado.");
                var doc = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = pDetail.DocumentId });
                ViewBag.Document = doc;
                await CargarListasAsync(pDetail.DocumentId);
                return View(pDetail);
            }

            if (!ModelState.IsValid)
            {
                var document = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = pDetail.DocumentId });
                ViewBag.Document = document;
                await CargarListasAsync(pDetail.DocumentId);
                return View(pDetail);
            }

            try
            {
                await DocumentDetailBL.GuardarAsync(pDetail, userId);
                TempData["Success"] = "Línea de detalle agregada correctamente.";
                return RedirectToAction("Details", "Documents",
                    new { id = pDetail.DocumentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var document = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = pDetail.DocumentId });
                ViewBag.Document = document;
                await CargarListasAsync(pDetail.DocumentId);
                return View(pDetail);
            }
        }

        // GET: DocumentDetails/Edit/5
        /// <summary>
        /// Muestra el formulario para editar una línea de detalle existente.
        /// </summary>
        /// <param name="id">Identificador del detalle a editar.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var detail = await DocumentDetailBL.ObtenerPorIdAsync(
                    new DocumentDetail { DocDetailId = id.Value });

                if (detail == null) return NotFound();

                var document = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = detail.DocumentId });

                ViewBag.Document = document;
                await CargarListasAsync(detail.DocumentId);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Documents");
            }
        }

        // POST: DocumentDetails/Edit/5
        /// <summary>
        /// Procesa la modificación de una línea de detalle existente.
        /// Recalcula subtotal, impuesto y total mediante la capa BL.
        /// </summary>
        /// <param name="id">Identificador del detalle proveniente de la ruta.</param>
        /// <param name="pDetail">Entidad <see cref="DocumentDetail"/> con los nuevos valores.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("DocDetailId,DocumentId,ProductId,Quantity,UnitPrice,DiscountAmount,TaxPercentage,Notes")]
            DocumentDetail pDetail)
        {
            if (id != pDetail.DocDetailId) return NotFound();

            if (!ModelState.IsValid)
            {
                var doc = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = pDetail.DocumentId });
                ViewBag.Document = doc;
                await CargarListasAsync(pDetail.DocumentId);
                return View(pDetail);
            }

            try
            {
                await DocumentDetailBL.ModificarAsync(pDetail);
                TempData["Success"] = "Línea de detalle modificada correctamente.";
                return RedirectToAction("Details", "Documents",
                    new { id = pDetail.DocumentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var doc = await DocumentBL.ObtenerPorIdAsync(
                    new Document { DocumentId = pDetail.DocumentId });
                ViewBag.Document = doc;
                await CargarListasAsync(pDetail.DocumentId);
                return View(pDetail);
            }
        }

        // GET: DocumentDetails/Delete/5
        /// <summary>
        /// Muestra la confirmación para eliminar físicamente una línea de detalle.
        /// </summary>
        /// <param name="id">Identificador del detalle a eliminar.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var detail = await DocumentDetailBL.ObtenerPorIdAsync(
                    new DocumentDetail { DocDetailId = id.Value });

                if (detail == null) return NotFound();
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Documents");
            }
        }

        // POST: DocumentDetails/Delete/5
        /// <summary>
        /// Elimina físicamente una línea de detalle, revierte el movimiento de inventario
        /// asociado y recalcula el total del documento padre dentro de una transacción.
        /// </summary>
        /// <param name="id">Identificador del detalle a eliminar.</param>
        /// <param name="documentId">Identificador del documento padre para la redirección.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int documentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                TempData["Error"] = "No se pudo identificar al usuario autenticado.";
                return RedirectToAction("Details", "Documents", new { id = documentId });
            }

            try
            {
                await DocumentDetailBL.EliminarAsync(
                    new DocumentDetail { DocDetailId = id }, userId);

                TempData["Success"] = "Línea de detalle eliminada y stock revertido correctamente.";
                return RedirectToAction("Details", "Documents", new { id = documentId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Documents", new { id = documentId });
            }
        }

        // ── Métodos Privados ─────────────────────────────────────────────────────

        /// <summary>
        /// Carga la lista de productos activos para el control desplegable de las vistas Create y Edit.
        /// </summary>
        /// <param name="documentId">Identificador del documento padre (para contexto en la vista).</param>
        private async Task CargarListasAsync(int documentId)
        {
            ViewBag.ProductList = new SelectList(
                await ProductListDAL.ObtenerTodosAsync(new ProductList { StatusId = 1 }),
                "ProductId", "Name");

            ViewData["DocumentId"] = documentId;
        }
    }
}