using SysGestionVentas.ApiClient.DTOs.Producto;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IProductoApiService
    {
        Task<List<ProductoSalida>> ObtenerTodosAsync();
        Task<ProductoSalida?> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(ProductoGuardar dto);
        Task<bool> ModificarAsync(ProductoModificar dto);
        Task<bool> EliminarAsync(int id);
    }
}