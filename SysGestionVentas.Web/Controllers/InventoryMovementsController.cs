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
    [Authorize(Roles = "Administrador")]
    public class InventoryMovementsController : Controller
    {
        // GET: InventoryMovements
        /// <summary>
        /// Muestra la lista paginada de movimientos de inventario con soporte de filtros.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="inventoryId">Filtro opcional por inventario.</param>
        /// <param name="movementTypeId">Filtro opcional por tipo de movimiento.</param>
        public async Task<IActionResult> Index(int page = 1, int inventoryId = 0, int movementTypeId = 0)
        {
            try
            {
                var query = new PagedQuery<InventoryMovement>
                {
                    Filter = new InventoryMovement
                    {
                        InventoryId = inventoryId,
                        MovementTypeId = movementTypeId
                    },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await InventoryMovementBL.BuscarAsync(query);

                await CargarFiltrosAsync(inventoryId, movementTypeId);

                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(new PagedResult<InventoryMovement>
                {
                    Items = new List<InventoryMovement>()
                });
            }
        }

        // GET: InventoryMovements/Details/5
        /// <summary>
        /// Muestra el detalle de un movimiento de inventario específico.
        /// </summary>
        /// <param name="id">Identificador del movimiento a consultar.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var movement = await InventoryMovementBL.ObtenerPorIdAsync(
                    new InventoryMovement { InventoryMovementId = id.Value });

                if (movement == null) return NotFound();
                return View(movement);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InventoryMovements/Create
        /// <summary>
        /// Muestra el formulario para registrar un nuevo movimiento de inventario.
        /// El campo <c>CreatedByUser</c> se toma automáticamente del usuario autenticado.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new InventoryMovement());
        }

        // POST: InventoryMovements/Create
        /// <summary>
        /// Procesa el registro de un nuevo movimiento de inventario.
        /// Actualiza el stock del inventario asociado de forma atómica.
        /// El tipo de movimiento determina si el stock incrementa, decrementa o se ajusta.
        /// </summary>
        /// <param name="pMovement">Entidad <see cref="InventoryMovement"/> con los datos del formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MovementTypeId,Quantity,UnitCost,Notes,InventoryId")]
            InventoryMovement pMovement)
        {
            // Asignar el usuario autenticado como creador del movimiento
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado.");
                await CargarListasAsync();
                return View(pMovement);
            }

            pMovement.CreatedByUser = userId;

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(pMovement);
            }

            try
            {
                await InventoryMovementBL.RegistrarMovimientoAsync(pMovement);
                TempData["Success"] = "Movimiento registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return View(pMovement);
            }
        }

        // ── Métodos Privados ─────────────────────────────────────────────────────

        /// <summary>
        /// Carga las listas de inventarios y tipos de movimiento
        /// necesarias para los controles desplegables de la vista Create.
        /// </summary>
        private async Task CargarListasAsync()
        {
            var inventories = await InventoryDAL.ObtenerTodosAsync(new Inventory { StatusId = 1 });
            ViewBag.InventoryList = new SelectList(
                inventories.Select(i => new
                {
                    i.InventoryId,
                    Nombre = i.Product != null ? i.Product.Name : $"Inventario #{i.InventoryId}"
                }),
                "InventoryId", "Nombre");

            ViewBag.MovementTypeList = new SelectList(
                await MovementTypeDAL.ObtenerTodosAsync(new MovementType(), pIsActive: true),
                "MovementTypeId", "Name");
        }

        /// <summary>
        /// Carga los filtros desplegables para la vista Index manteniendo las selecciones actuales.
        /// </summary>
        /// <param name="inventoryId">ID de inventario actualmente filtrado.</param>
        /// <param name="movementTypeId">ID de tipo de movimiento actualmente filtrado.</param>
        private async Task CargarFiltrosAsync(int inventoryId, int movementTypeId)
        {
            var inventories = await InventoryDAL.ObtenerTodosAsync(new Inventory());
            ViewBag.InventoryList = new SelectList(
                inventories.Select(i => new
                {
                    i.InventoryId,
                    Nombre = i.Product != null ? i.Product.Name : $"Inventario #{i.InventoryId}"
                }),
                "InventoryId", "Nombre", inventoryId);

            ViewBag.MovementTypeList = new SelectList(
                await MovementTypeDAL.ObtenerTodosAsync(new MovementType()),
                "MovementTypeId", "Name", movementTypeId);
        }
    }
}