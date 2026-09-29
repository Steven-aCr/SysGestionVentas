using SysGestionVentas.ApiClient.DTOs.Auth;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest);
    }
}