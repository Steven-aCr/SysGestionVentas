using SysGestionVentas.ApiClient.DTOs.Producto;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IProductoApiService
    {
        Task<List<ProductoSalida>> ObtenerTodosAsync();

        Task<ProductoSalida?> ObtenerPorIdAsync(int id);

        Task<List<ProductoSalida>> ObtenerPorCategoriaAsync(
            int idCategoria);

        Task<ProductoSalida?> CrearAsync(
            ProductoGuardar dto);

        Task<ProductoSalida?> ModificarAsync(
            ProductoModificar dto);
    }
}