using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DbContexto _context;

        public DocumentController(DbContexto context)
        {
            _context = context;
        }

        // GET: Document
        public async Task<IActionResult> Index()
        {
            var documents = await _context.Document
                .Include(d => d.DocumentType)
                .Include(d => d.Person)
                .ToListAsync();
            return View(documents);
        }

        // GET: Document/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var document = await _context.Document
                .Include(d => d.DocumentType)
                .Include(d => d.Person)
                .FirstOrDefaultAsync(d => d.DocumentId == id);

            if (document == null)
                return NotFound();

            return View(document);
        }

        // GET: Document/Create
        public IActionResult Create()
        {
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name");
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "FirstName");
            return View();
        }

        // POST: Document/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentId,DocTypeId,DocNumber,IssueDate,PersonId")] Document pDocument)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await DocumentBL.GuardarAsync(pDocument);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", pDocument.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "FirstName", pDocument.PersonId);
            return View(pDocument);
        }

        // GET: Document/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var document = await _context.Document.FindAsync(id);
            if (document == null)
                return NotFound();

            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", document.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "FirstName", document.PersonId);
            return View(document);
        }

        // POST: Document/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,DocTypeId,DocNumber,IssueDate,PersonId")] Document pDocument)
        {
            if (id != pDocument.DocumentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await DocumentBL.ModificarAsync(pDocument);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", pDocument.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "FirstName", pDocument.PersonId);
            return View(pDocument);
        }

        // GET: Document/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var document = await _context.Document
                .Include(d => d.DocumentType)
                .Include(d => d.Person)
                .FirstOrDefaultAsync(d => d.DocumentId == id);

            if (document == null)
                return NotFound();

            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = new Document
            {
                DocumentId = id,
                StatusId = 2 // Estado inactivo
            };
            try
            {
                await DocumentBL.EliminarAsync(document);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(d => d.DocumentId == id);
        }
    }
}