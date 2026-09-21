using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Categoria;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class CategoriaApiService : ICategoriaApiService
    {
        private readonly HttpClient _httpClient;

        public CategoriaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoriaSalida>> ObtenerTodosAsync()
        {
            var respuesta = await _httpClient.GetFromJsonAsync<List<CategoriaSalida>>("api/categoria");
            return respuesta ?? new List<CategoriaSalida>();
        }

        public async Task<CategoriaSalida?> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CategoriaSalida>($"api/categoria/{id}");
        }

        public async Task<bool> CrearAsync(CategoriaGuardar dto)
        {
            var respuesta = await _httpClient.PostAsJsonAsync("api/categoria", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> ModificarAsync(CategoriaModificar dto)
        {
            var respuesta = await _httpClient.PutAsJsonAsync($"api/categoria/{dto.Id}", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var respuesta = await _httpClient.DeleteAsync($"api/categoria/{id}");
            return respuesta.IsSuccessStatusCode;
        }
    }
}