using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class DocumentTypeController : Controller
    {
        private readonly DocumentTypeBL _documentTypeBL;

        // Constructor con inyección de dependencias
        public DocumentTypeController(DocumentTypeBL documentTypeBL)
        {
            _documentTypeBL = documentTypeBL ?? throw new ArgumentNullException(nameof(documentTypeBL));
        }

        // GET: DocumentType
        public async Task<IActionResult> Index()
        {
            try
            {
                var documentTypes = await _documentTypeBL.ObtenerTodosAsync();
                return View(documentTypes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<DocumentType>());
            }
        }

        // GET: DocumentType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var documentType = await _documentTypeBL.ObtenerPorIdAsync(id.Value);
                if (documentType == null)
                    return NotFound();
                return View(documentType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: DocumentType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentType documentType)
        {
            if (!ModelState.IsValid)
                return View(documentType);
            try
            {
                await _documentTypeBL.GuardarAsync(documentType);
                TempData["Success"] = "Tipo de documento creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(documentType);
            }
        }

        // GET: DocumentType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var documentType = await _documentTypeBL.ObtenerPorIdAsync(id.Value);
                if (documentType == null)
                    return NotFound();
                return View(documentType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: DocumentType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentType documentType)
        {
            if (id != documentType.DocTypeId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(documentType);
            try
            {
                await _documentTypeBL.ModificarAsync(documentType);
                TempData["Success"] = "Tipo de documento modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(documentType);
            }
        }

        // GET: DocumentType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var documentType = await _documentTypeBL.ObtenerPorIdAsync(id.Value);
                if (documentType == null)
                    return NotFound();
                return View(documentType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: DocumentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _documentTypeBL.EliminarAsync(id);
                TempData["Success"] = "Tipo de documento eliminado correctamente.";
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
