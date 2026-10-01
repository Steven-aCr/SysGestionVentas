using SysGestionVentas.ApiClient.DTOs.Inventario;
namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IInventarioApiService
    {
        Task<List<InventarioSalida>> ObtenerTodosAsync();
        Task<InventarioSalida?> ObtenerPorProductoAsync(int idProducto);
        Task<InventarioSalida?> CrearAsync(InventarioGuardar dto);
        Task<InventarioSalida?> ModificarAsync(InventarioModificar dto);
    }
}