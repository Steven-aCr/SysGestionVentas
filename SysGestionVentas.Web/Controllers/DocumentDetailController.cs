using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    public class DocumentDetailController : Controller
    {
        private readonly DbContexto _context;

        public DocumentDetailController(DbContexto context)
        {
            _context = context;
        }

        // GET: DocumentDetail
        public async Task<IActionResult> Index()
        {
            var details = await _context.DocumentDetail
                .Include(d => d.Document)
                .Include(d => d.ProductList)
                .ToListAsync();
            return View(details);
        }

        // GET: DocumentDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.DocumentDetail
                .Include(d => d.Document)
                .Include(d => d.ProductList)
                .FirstOrDefaultAsync(d => d.DocDetailId == id);

            if (detail == null)
                return NotFound();

            return View(detail);
        }

        // GET: DocumentDetail/Create
        public IActionResult Create()
        {
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber");
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name");
            return View();
        }

        // POST: DocumentDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocDetailId,DocumentId,ProductId,Quantity,UnitPrice,Subtotal")] DocumentDetail pDocumentDetail)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await DocumentDetailBL.GuardarAsync(pDocumentDetail);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", pDocumentDetail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", pDocumentDetail.ProductId);
            return View(pDocumentDetail);
        }

        // GET: DocumentDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.DocumentDetail.FindAsync(id);
            if (detail == null)
                return NotFound();

            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", detail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", detail.ProductId);
            return View(detail);
        }

        // POST: DocumentDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocDetailId,DocumentId,ProductId,Quantity,UnitPrice,Subtotal")] DocumentDetail pDocumentDetail)
        {
            if (id != pDocumentDetail.DocDetailId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await DocumentDetailBL.ModificarAsync(pDocumentDetail);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", pDocumentDetail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Name", pDocumentDetail.ProductId);
            return View(pDocumentDetail);
        }

        // GET: DocumentDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.DocumentDetail
                .Include(d => d.Document)
                .Include(d => d.ProductList)
                .FirstOrDefaultAsync(d => d.DocDetailId == id);

            if (detail == null)
                return NotFound();

            return View(detail);
        }

        // POST: DocumentDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detail = new DocumentDetail { DocDetailId = id };
            try
            {
                await DocumentDetailBL.EliminarAsync(detail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentDetailExists(int id)
        {
            return _context.DocumentDetail.Any(d => d.DocDetailId == id);
        }
    }
}