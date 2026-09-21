using SysGestionVentas.ApiClient.DTOs.Categoria;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface ICategoriaApiService
    {
        Task<List<CategoriaSalida>> ObtenerTodosAsync();
        Task<CategoriaSalida?> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(CategoriaGuardar dto);
        Task<bool> ModificarAsync(CategoriaModificar dto);
        Task<bool> EliminarAsync(int id);
    }
}