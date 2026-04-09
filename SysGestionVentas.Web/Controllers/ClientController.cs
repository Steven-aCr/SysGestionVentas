using BDGestionVentas.BL; // Asegúrate de que este espacio de nombres sea correcto
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly ClientBL _clientBL;

        // Constructor con inyección de dependencias
        public ClientController(ClientBL clientBL)
        {
            _clientBL = clientBL ?? throw new ArgumentNullException(nameof(clientBL));
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            try
            {
                var clients = await _clientBL.ObtenerTodosAsync();
                return View(clients);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Client>());
            }
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var client = await _clientBL.ObtenerPorIdAsync(id.Value);
                if (client == null)
                    return NotFound();
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);
            try
            {
                await _clientBL.GuardarAsync(client);
                TempData["Success"] = "Cliente creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(client);
            }
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var client = await _clientBL.ObtenerPorIdAsync(id.Value);
                if (client == null)
                    return NotFound();
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId)
                return NotFound();
            if (!ModelState.IsValid)
                return View(client);
            try
            {
                await _clientBL.ModificarAsync(client);
                TempData["Success"] = "Cliente modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(client);
            }
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var client = await _clientBL.ObtenerPorIdAsync(id.Value);
                if (client == null)
                    return NotFound();
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _clientBL.EliminarAsync(id);
                TempData["Success"] = "Cliente eliminado correctamente.";
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
