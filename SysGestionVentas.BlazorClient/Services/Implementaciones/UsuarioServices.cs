using SysGestionVentas.ApiClient.Services.Interfaces;
using SysGestionVentas.BlazorClient.DTOs;
using System.Net.Http.Json;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class UsuarioApiServices : IUsuarioApiServices
    {
        private readonly HttpClient _http;

        public UsuarioApiServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UsuarioDto>?> GetUsuariosAsync()
        {
            return await _http.GetFromJsonAsync<List<UsuarioDto>>("api/usuario");
        }
    }
}