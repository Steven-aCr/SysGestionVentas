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
    public class DocumentsController : Controller
    {
        private readonly DbContexto _context;

        public DocumentsController(DbContexto context)
        {
            _context = context;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var dbContexto = _context.Document.Include(d => d.CreatedBy).Include(d => d.DocumentType).Include(d => d.Person).Include(d => d.Status);
            return View(await dbContexto.ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .Include(d => d.CreatedBy)
                .Include(d => d.DocumentType)
                .Include(d => d.Person)
                .Include(d => d.Status)
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email");
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name");
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Adress");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentId,DocTypeId,DocNumber,IssueDate,TotalAmount,PersonId,CreatedByUser,StatusId")] Document document)
        {
            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", document.CreatedByUser);
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", document.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Adress", document.PersonId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name", document.StatusId);
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", document.CreatedByUser);
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", document.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Adress", document.PersonId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name", document.StatusId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,DocTypeId,DocNumber,IssueDate,TotalAmount,PersonId,CreatedByUser,StatusId")] Document document)
        {
            if (id != document.DocumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentId))
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
            ViewData["CreatedByUser"] = new SelectList(_context.User, "UserId", "Email", document.CreatedByUser);
            ViewData["DocTypeId"] = new SelectList(_context.DocumentType, "DocTypeId", "Name", document.DocTypeId);
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Adress", document.PersonId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name", document.StatusId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .Include(d => d.CreatedBy)
                .Include(d => d.DocumentType)
                .Include(d => d.Person)
                .Include(d => d.Status)
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Document.FindAsync(id);
            if (document != null)
            {
                _context.Document.Remove(document);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(e => e.DocumentId == id);
        }

        private async Task CargarListaAsync()
        {
         ViewBag.DocumentTypes = new SelectList(
             await DocumentTypeDAL.ObtenerTodosAsync(new DocumentType { StatusId =1}),
             "DocTypeId", "Name");
            ViewBag.StatusList = new SelectList(
                await StatusDAL.ObtenerPorTiposAsync(new List<int> { 3 }, pIsActive: true),
                "StatusId", "Name");
        }
    }
}
