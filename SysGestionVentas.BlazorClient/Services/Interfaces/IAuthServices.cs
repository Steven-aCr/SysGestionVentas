using SysGestionVentas.BlazorClient.DTOs;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<UsuarioTokenDTOs?> LoginAsync(LoginDTOs loginDTOs);
    }
}