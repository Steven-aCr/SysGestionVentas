using SysGestionVentas.BlazorClient.DTOs;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IUsuarioApiServices
    {
        Task<List<UsuarioDto>?> GetUsuariosAsync();
    }
}