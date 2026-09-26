using SysGestionVentas.BlazorClient.DTOs;

namespace SysGestionVentas.BlazorClient.Services.Interfaces
{
    public interface IProductoServices
    {
        Task<List<ProductoDto>?> GetProductosAsync();
        Task<ProductoDto?> GetProductoByIdAsync(long id);
        Task<bool> CrearProductoAsync(ProductoDto producto);
        Task<bool> EliminarProductoAsync(long id);
    }
}