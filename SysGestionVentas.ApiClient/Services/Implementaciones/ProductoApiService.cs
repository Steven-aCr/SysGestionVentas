using System.Net;
using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Producto;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class ProductoApiService : IProductoApiService
    {
        private const string Endpoint = "api/productos";

        private readonly HttpClient _httpClient;

        public ProductoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductoSalida>> ObtenerTodosAsync()
        {
            using var respuesta =
                await _httpClient.GetAsync(Endpoint);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                       .ReadFromJsonAsync<List<ProductoSalida>>()
                   ?? new List<ProductoSalida>();
        }

        public async Task<ProductoSalida?> ObtenerPorIdAsync(int id)
        {
            using var respuesta =
                await _httpClient.GetAsync($"{Endpoint}/{id}");

            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<ProductoSalida>();
        }

        public async Task<List<ProductoSalida>>
            ObtenerPorCategoriaAsync(int idCategoria)
        {
            using var respuesta =
                await _httpClient.GetAsync(
                    $"{Endpoint}/categoria/{idCategoria}");

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                       .ReadFromJsonAsync<List<ProductoSalida>>()
                   ?? new List<ProductoSalida>();
        }

        public async Task<ProductoSalida?> CrearAsync(
            ProductoGuardar dto)
        {
            using var respuesta =
                await _httpClient.PostAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<ProductoSalida>();
        }

        public async Task<ProductoSalida?> ModificarAsync(
            ProductoModificar dto)
        {
            using var respuesta =
                await _httpClient.PutAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<ProductoSalida>();
        }
    }
}