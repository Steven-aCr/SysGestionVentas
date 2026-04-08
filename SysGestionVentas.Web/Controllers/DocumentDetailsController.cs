using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrador,Vendedor")]
    public class DocumentDetailsController : Controller
    {
        private readonly DbContexto _context;

        public DocumentDetailsController(DbContexto context)
        {
            _context = context;
        }

        // GET: DocumentDetails
        public async Task<IActionResult> Index()
        {
            var dbContexto = _context.DocumentDetail.Include(d => d.Document).Include(d => d.Product);
            return View(await dbContexto.ToListAsync());
        }

        // GET: DocumentDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentDetail = await _context.DocumentDetail
                .Include(d => d.Document)
                .Include(d => d.Product)
                .FirstOrDefaultAsync(m => m.DocDetailId == id);
            if (documentDetail == null)
            {
                return NotFound();
            }

            return View(documentDetail);
        }

        // GET: DocumentDetails/Create
        public IActionResult Create()
        {
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber");
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Barcode");
            return View();
        }

        // POST: DocumentDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocDetailId,DocumentId,ProductId,Quantity,UnitPrice,DiscountAmount,Subtotal,TaxPercentage,TaxAmount,TotalAmount,Notes")] DocumentDetail documentDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documentDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", documentDetail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Barcode", documentDetail.ProductId);
            return View(documentDetail);
        }

        // GET: DocumentDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentDetail = await _context.DocumentDetail.FindAsync(id);
            if (documentDetail == null)
            {
                return NotFound();
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", documentDetail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Barcode", documentDetail.ProductId);
            return View(documentDetail);
        }

        // POST: DocumentDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocDetailId,DocumentId,ProductId,Quantity,UnitPrice,DiscountAmount,Subtotal,TaxPercentage,TaxAmount,TotalAmount,Notes")] DocumentDetail documentDetail)
        {
            if (id != documentDetail.DocDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documentDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentDetailExists(documentDetail.DocDetailId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocNumber", documentDetail.DocumentId);
            ViewData["ProductId"] = new SelectList(_context.ProductList, "ProductId", "Barcode", documentDetail.ProductId);
            return View(documentDetail);
        }

        // GET: DocumentDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentDetail = await _context.DocumentDetail
                .Include(d => d.Document)
                .Include(d => d.Product)
                .FirstOrDefaultAsync(m => m.DocDetailId == id);
            if (documentDetail == null)
            {
                return NotFound();
            }

            return View(documentDetail);
        }

        // POST: DocumentDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentDetail = await _context.DocumentDetail.FindAsync(id);
            if (documentDetail != null)
            {
                _context.DocumentDetail.Remove(documentDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentDetailExists(int id)
        {
            return _context.DocumentDetail.Any(e => e.DocDetailId == id);
        }

        private async Task CargarListasAsync()
        {
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 1, 3 }, pIsActive: true), 
                "StatusId", "Name");
        }
    }
}
