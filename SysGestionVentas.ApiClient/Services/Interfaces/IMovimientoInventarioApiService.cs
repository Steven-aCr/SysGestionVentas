using SysGestionVentas.ApiClient.DTOs.MovimientoInventario;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IMovimientoInventarioApiService
    {
        Task<List<MovimientoInventarioSalida>> ObtenerTodosAsync();

        Task<MovimientoInventarioSalida?> ObtenerPorIdAsync(int id);

        Task<List<MovimientoInventarioSalida>> ObtenerPorInventarioAsync(
            int idInventario);

        Task<MovimientoInventarioSalida?> GuardarAsync(
            MovimientoInventarioGuardar movimiento);
    }
}