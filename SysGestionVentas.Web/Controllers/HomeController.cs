using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.Web.Models;
using System.Diagnostics;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Inicializa el controlador con el servicio de logging inyectado.
        /// </summary>
        /// <param name="logger">Instancia del servicio de registro de eventos.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: Home/Index
        /// <summary>
        /// Acción principal del Dashboard de Administración.
        /// Agrega y presenta las métricas clave del sistema: ventas del mes,
        /// estado de inventario, distribución de usuarios, empleados por departamento
        /// y actividad reciente de movimientos de inventario.
        /// Solo accesible por el rol Administrador.
        /// </summary>
        /// <returns>
        /// Vista del dashboard con el <see cref="AdminDashboardModel"/> poblado,
        /// o vista con modelo vacío si ocurre un error en la carga de datos.
        /// </returns>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vm = new AdminDashboardModel();
                var ahora = DateTime.UtcNow;
                var inicioMesActual = new DateTime(ahora.Year, ahora.Month, 1);
                var inicioMesAnterior = inicioMesActual.AddMonths(-1);

                // ── Documentos / Ventas ─────────────────────────────────────────
                var todosDocumentos = await DocumentDAL.ObtenerTodosAsync(new Document());

                var docsMesActual = todosDocumentos
                    .Where(d => d.IssueDate >= inicioMesActual).ToList();

                var docsMesAnterior = todosDocumentos
                    .Where(d => d.IssueDate >= inicioMesAnterior && d.IssueDate < inicioMesActual)
                    .ToList();

                vm.VentasMesActual = docsMesActual.Count;
                vm.VentasMesAnterior = docsMesAnterior.Count;
                vm.MontoTotalMes = docsMesActual.Sum(d => d.TotalAmount);
                vm.MontoTotalMesAnterior = docsMesAnterior.Sum(d => d.TotalAmount);

                vm.UltimosDocumentos = todosDocumentos
                    .OrderByDescending(d => d.IssueDate)
                    .Take(8)
                    .ToList();

                // ── Ventas por mes (últimos 6 meses) ───────────────────────────
                vm.VentasPorMes = Enumerable.Range(0, 6)
                    .Select(i =>
                    {
                        var mes = inicioMesActual.AddMonths(-i);
                        var inicioMes = new DateTime(mes.Year, mes.Month, 1);
                        var finMes = inicioMes.AddMonths(1);
                        var docsDelMes = todosDocumentos
                            .Where(d => d.IssueDate >= inicioMes && d.IssueDate < finMes)
                            .ToList();
                        return new VentaMensual
                        {
                            Mes = mes.ToString("MMM", new System.Globalization.CultureInfo("es-ES")),
                            Monto = docsDelMes.Sum(d => d.TotalAmount),
                            Cantidad = docsDelMes.Count
                        };
                    })
                    .Reverse()
                    .ToList();

                // ── Inventario ──────────────────────────────────────────────────
                var inventarios = await InventoryDAL.ObtenerTodosAsync(new Inventory());
                vm.TotalProductos = inventarios.Count;
                vm.ProductosBajoStock = inventarios.Count(i => i.CurrentStock <= i.MinimumStock);
                vm.InventarioCritico = inventarios
                    .Where(i => i.CurrentStock <= i.MinimumStock)
                    .OrderBy(i => i.CurrentStock)
                    .Take(5)
                    .ToList();

                // ── Usuarios ────────────────────────────────────────────────────
                var todosUsuarios = await UserDAL.ObtenerTodosAsync(new User());
                vm.TotalUsuarios = todosUsuarios.Count;
                vm.TotalUsuariosActivos = todosUsuarios.Count(u => u.Status?.Name == "Activo");

                var coloresPorRol = new Dictionary<string, string>
                {
                    { "Administrador", "#0d9488" },
                    { "Vendedor",      "#6366f1" },
                    { "Cliente",       "#f59e0b" },
                    { "Gerente",       "#ec4899" }
                };

                vm.DistribucionPorRol = todosUsuarios
                    .GroupBy(u => u.Rol?.Name ?? "Sin Rol")
                    .Select(g => new DistribucionRol
                    {
                        Rol = g.Key,
                        Cantidad = g.Count(),
                        Color = coloresPorRol.TryGetValue(g.Key, out var c) ? c : "#9ca3af"
                    })
                    .OrderByDescending(d => d.Cantidad)
                    .ToList();

                // ── Clientes ────────────────────────────────────────────────────
                // FIX: Se pasa Person inicializado para evitar NullReferenceException en el DAL
                var todosClientes = await ClientDAL.ObtenerTodosAsync(new Client
                {
                    Person = new Person { StatusId = 0 }
                });
                vm.TotalClientes = todosClientes.Count;
                vm.TotalClientesActivos = todosClientes
                    .Count(c => c.Person?.Status?.Name == "Activo");

                // ── Empleados ───────────────────────────────────────────────────
                var todosEmpleados = await EmployeeDAL.ObtenerTodosAsync(new Employee { StatusId = 1 });
                vm.TotalEmpleados = todosEmpleados.Count;

                vm.EmpleadosPorDepartamento = todosEmpleados
                    .GroupBy(e => e.Department?.Name ?? "Sin Departamento")
                    .Select(g => new EmpleadosPorDepartamento
                    {
                        Departamento = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(d => d.Cantidad)
                    .ToList();

                // ── Últimos movimientos de inventario ──────────────────────────
                var movimientos = await InventoryMovementDAL.ObtenerTodosAsync(
                    new InventoryMovement(),
                    pFromDate: ahora.AddDays(-30));

                vm.UltimosMovimientos = movimientos
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(6)
                    .ToList();

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard de administración.");
                TempData["Error"] = "No se pudieron cargar los datos del dashboard. Intente nuevamente.";
                return View(new AdminDashboardModel());
            }
        }

        // GET: Home/Privacy
        /// <summary>
        /// Muestra la página de política de privacidad del sistema.
        /// </summary>
        public IActionResult Privacy() => View();

        // GET: Home/Error
        /// <summary>
        /// Muestra la vista de error genérica del sistema.
        /// Se invoca automáticamente por el middleware de manejo de excepciones.
        /// </summary>
        /// <returns>Vista de error con el RequestId para trazabilidad.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}