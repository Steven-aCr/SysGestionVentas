using System.Net.Http.Json;
using SysGestionVentas.BlazorClient.DTOs;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _http;

        public AuthApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UsuarioTokenDTOs?> LoginAsync(LoginDTOs loginDTOs)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", loginDTOs);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UsuarioTokenDTOs>();
            }

            return null;
        }
    }
}