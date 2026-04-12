using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]


    /// <summary>
    /// Controlador del área de Ventas, accesible únicamente por usuarios
    /// con rol "Administrador" o "Vendedor".
    /// El vendedor tiene acceso de solo lectura a documentos, productos
    /// e inventario. No puede crear ni modificar tipos de documento.
    /// </summary>
    [Authorize(Roles = "Administrador,Vendedor")]
    public class VentasController : Controller
    {
        // ── GET: Ventas/Dashboard ─────────────────────────────────

        /// <summary>
        /// Muestra el dashboard principal del área de ventas con
        /// los últimos documentos registrados en el sistema.
        /// </summary>
        /// <returns>Vista del dashboard con los documentos más recientes.</returns>
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var documentos = await DocumentDAL.ObtenerTodosAsync(new Document());
                return View(documentos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Document>());
            }
        }

        // ── GET: Ventas/Documentos ────────────────────────────────

        /// <summary>
        /// Lista todos los documentos del sistema con soporte de búsqueda
        /// y paginación. Solo lectura; no expone acciones de creación.
        /// </summary>
        /// <param name="page">Número de página actual (por defecto: 1).</param>
        /// <param name="busqueda">Texto de búsqueda parcial sobre el número de documento.</param>
        /// <returns>Vista paginada de documentos.</returns>
        public async Task<IActionResult> Documentos(int page = 1, string? busqueda = null)
        {
            try
            {
                var query = new PagedQuery<Document>
                {
                    Filter = new Document { DocNumber = busqueda },
                    Page = page,
                    PageSize = 20
                };

                var resultado = await DocumentDAL.BuscarAsync(query);
                ViewBag.Busqueda = busqueda;
                return View(resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new PagedResult<Document>());
            }
        }

        // ── GET: Ventas/DetalleDocumento/5 ───────────────────────

        /// <summary>
        /// Muestra el detalle completo de un documento específico,
        /// incluyendo sus líneas de detalle.
        /// </summary>
        /// <param name="id">Identificador del documento a consultar.</param>
        /// <returns>Vista de detalle del documento o NotFound si no existe.</returns>
        public async Task<IActionResult> DetalleDocumento(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var documento = await DocumentDAL.ObtenerPorIdAsync(new Document { DocumentId = id.Value });
                if (documento == null)
                    return NotFound();

                var detalles = await DocumentDetailDAL.ObtenerPorDocumentoAsync(id.Value);
                ViewBag.Detalles = detalles;
                return View(documento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Documentos));
            }
        }

        // ── GET: Ventas/Productos ─────────────────────────────────

        /// <summary>
        /// Lista todos los productos activos con su información de inventario.
        /// Solo lectura; el vendedor no puede crear ni modificar productos.
        /// </summary>
        /// <returns>Vista de catálogo de productos.</returns>
        public async Task<IActionResult> Productos()
        {
            try
            {
                var productos = await ProductListDAL.ObtenerTodosAsync(new ProductList());
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<ProductList>());
            }
        }
    }
}