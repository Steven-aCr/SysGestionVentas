using SysGestionVentas.BlazorClient.DTOs;
using static SysGestionVentas.BlazorClient.Pages.Categoria;

namespace SysGestionVentas.BlazorClient.Services.Interfaces
{
    public interface ICategoriaServices
    {
        Task<List<CategoriaDto>?> GetCategoriasAsync();
        Task<bool> CrearCategoriaAsync(CategoriaDto categoria);
    }
}