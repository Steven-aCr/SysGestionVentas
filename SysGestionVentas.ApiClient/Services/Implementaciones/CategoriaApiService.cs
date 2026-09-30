using System.Net;
using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Categoria;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class CategoriaApiService : ICategoriaApiService
    {
        private const string Endpoint = "api/categorias";

        private readonly HttpClient _httpClient;

        public CategoriaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoriaSalida>> ObtenerTodosAsync()
        {
            using var respuesta = await _httpClient.GetAsync(Endpoint);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                       .ReadFromJsonAsync<List<CategoriaSalida>>()
                   ?? new List<CategoriaSalida>();
        }

        public async Task<CategoriaSalida?> ObtenerPorIdAsync(int id)
        {
            using var respuesta =
                await _httpClient.GetAsync($"{Endpoint}/{id}");

            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<CategoriaSalida>();
        }

        public async Task<CategoriaSalida?> CrearAsync(
            CategoriaGuardar dto)
        {
            using var respuesta =
                await _httpClient.PostAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<CategoriaSalida>();
        }

        public async Task<CategoriaSalida?> ModificarAsync(
            CategoriaModificar dto)
        {
            using var respuesta =
                await _httpClient.PutAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<CategoriaSalida>();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            using var respuesta =
                await _httpClient.DeleteAsync($"{Endpoint}/{id}");

            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            respuesta.EnsureSuccessStatusCode();

            return true;
        }
    }
}