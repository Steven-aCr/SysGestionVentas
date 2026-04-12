using SysGestionVentas.EN;

namespace SysGestionVentas.Web.Models
{
    /// <summary>
    /// ViewModel principal del Dashboard de Administración.
    /// Agrega métricas clave del sistema para su presentación en la vista de inicio.
    /// Cubre flujos de ventas, inventario, usuarios, empleados y clientes.
    /// </summary>
    public class AdminDashboardModel
    {
        // ── KPIs de Ventas ──────────────────────────────────────────────────────

        /// <summary>Total de documentos/ventas emitidos en el mes actual.</summary>
        public int VentasMesActual { get; set; }

        /// <summary>Total de documentos/ventas emitidos en el mes anterior (para comparación).</summary>
        public int VentasMesAnterior { get; set; }

        /// <summary>Monto total facturado en el mes actual (suma de TotalAmount de documentos activos).</summary>
        public decimal MontoTotalMes { get; set; }

        /// <summary>Monto total facturado en el mes anterior.</summary>
        public decimal MontoTotalMesAnterior { get; set; }

        /// <summary>Lista de los últimos 10 documentos emitidos para mostrar en la tabla de actividad reciente.</summary>
        public List<Document> UltimosDocumentos { get; set; } = new();

        /// <summary>
        /// Datos mensuales de ventas para el gráfico de los últimos 6 meses.
        /// Key: nombre abreviado del mes (ej. "Ene"), Value: monto total.
        /// </summary>
        public List<VentaMensual> VentasPorMes { get; set; } = new();

        // ── KPIs de Inventario ──────────────────────────────────────────────────

        /// <summary>Total de productos registrados en el sistema.</summary>
        public int TotalProductos { get; set; }

        /// <summary>Número de productos con stock por debajo del stock mínimo configurado.</summary>
        public int ProductosBajoStock { get; set; }

        /// <summary>Lista de productos con stock crítico (CurrentStock menor que MinimumStock).</summary>
        public List<Inventory> InventarioCritico { get; set; } = new();

        // ── KPIs de Usuarios / Personas ─────────────────────────────────────────

        /// <summary>Total de usuarios activos en el sistema.</summary>
        public int TotalUsuariosActivos { get; set; }

        /// <summary>Total de usuarios registrados (incluyendo inactivos).</summary>
        public int TotalUsuarios { get; set; }

        /// <summary>Total de clientes registrados en el sistema.</summary>
        public int TotalClientes { get; set; }

        /// <summary>Total de clientes activos (con estado Activo en su Person asociada).</summary>
        public int TotalClientesActivos { get; set; }

        /// <summary>Distribución de usuarios por rol para el gráfico de anillo.</summary>
        public List<DistribucionRol> DistribucionPorRol { get; set; } = new();

        // ── KPIs de Empleados ───────────────────────────────────────────────────

        /// <summary>Total de empleados activos registrados.</summary>
        public int TotalEmpleados { get; set; }

        /// <summary>Total de empleados por departamento para la sección de RRHH.</summary>
        public List<EmpleadosPorDepartamento> EmpleadosPorDepartamento { get; set; } = new();

        // ── Actividad Reciente ──────────────────────────────────────────────────

        /// <summary>Lista de los últimos 5 movimientos de inventario registrados.</summary>
        public List<InventoryMovement> UltimosMovimientos { get; set; } = new();

        // ── Propiedades calculadas ──────────────────────────────────────────────

        /// <summary>
        /// Variación porcentual de ventas respecto al mes anterior.
        /// Retorna 0 si el mes anterior no tuvo ventas para evitar división por cero.
        /// </summary>
        public decimal VariacionVentas => VentasMesAnterior == 0
            ? (VentasMesActual > 0 ? 100 : 0)
            : Math.Round(((decimal)(VentasMesActual - VentasMesAnterior) / VentasMesAnterior) * 100, 1);

        /// <summary>
        /// Variación porcentual del monto facturado respecto al mes anterior.
        /// Retorna 0 si el mes anterior no tuvo facturación.
        /// </summary>
        public decimal VariacionMonto => MontoTotalMesAnterior == 0
            ? (MontoTotalMes > 0 ? 100 : 0)
            : Math.Round(((MontoTotalMes - MontoTotalMesAnterior) / MontoTotalMesAnterior) * 100, 1);

        /// <summary>Indica si la tendencia de ventas es positiva respecto al mes anterior.</summary>
        public bool TendenciaVentasPositiva => VentasMesActual >= VentasMesAnterior;

        /// <summary>Indica si la tendencia de monto facturado es positiva respecto al mes anterior.</summary>
        public bool TendenciaMontoPositiva => MontoTotalMes >= MontoTotalMesAnterior;
    }

    /// <summary>
    /// Registro de ventas por mes para el gráfico de tendencia de los últimos 6 meses.
    /// </summary>
    public class VentaMensual
    {
        /// <summary>Nombre abreviado del mes (ej. "Ene", "Feb").</summary>
        public string Mes { get; set; } = string.Empty;

        /// <summary>Monto total facturado en ese mes.</summary>
        public decimal Monto { get; set; }

        /// <summary>Número de documentos emitidos en ese mes.</summary>
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Distribución de usuarios por rol para el gráfico de anillo del panel de usuarios.
    /// </summary>
    public class DistribucionRol
    {
        /// <summary>Nombre del rol (ej. "Administrador", "Vendedor", "Cliente").</summary>
        public string Rol { get; set; } = string.Empty;

        /// <summary>Cantidad de usuarios con ese rol.</summary>
        public int Cantidad { get; set; }

        /// <summary>Color hexadecimal asignado al rol para el gráfico.</summary>
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// Conteo de empleados activos agrupados por departamento para la sección de RRHH.
    /// </summary>
    public class EmpleadosPorDepartamento
    {
        /// <summary>Nombre del departamento.</summary>
        public string Departamento { get; set; } = string.Empty;

        /// <summary>Número de empleados activos en ese departamento.</summary>
        public int Cantidad { get; set; }
    }
}