using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Producto;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class ProductoApiService : IProductoApiService
    {
        private readonly HttpClient _httpClient;

        public ProductoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductoSalida>> ObtenerTodosAsync()
        {
            var respuesta = await _httpClient.GetFromJsonAsync<List<ProductoSalida>>("api/producto");
            return respuesta ?? new List<ProductoSalida>();
        }

        public async Task<ProductoSalida?> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductoSalida>($"api/producto/{id}");
        }

        public async Task<bool> CrearAsync(ProductoGuardar dto)
        {
            var respuesta = await _httpClient.PostAsJsonAsync("api/producto", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> ModificarAsync(ProductoModificar dto)
        {
            var respuesta = await _httpClient.PutAsJsonAsync($"api/producto/{dto.Id}", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var respuesta = await _httpClient.DeleteAsync($"api/producto/{id}");
            return respuesta.IsSuccessStatusCode;
        }
    }
}