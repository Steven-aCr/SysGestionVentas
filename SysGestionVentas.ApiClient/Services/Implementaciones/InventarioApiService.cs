using System.Net;
using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Inventario;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class InventarioApiService : IInventarioApiService
    {
        private const string Endpoint = "api/inventario";
        private readonly HttpClient _httpClient;

        public InventarioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<InventarioSalida>> ObtenerTodosAsync()
        {
            using var respuesta = await _httpClient.GetAsync(Endpoint);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                       .ReadFromJsonAsync<List<InventarioSalida>>()
                   ?? new List<InventarioSalida>();
        }

        public async Task<InventarioSalida?> ObtenerPorProductoAsync(
            int idProducto)
        {
            using var respuesta =
                await _httpClient.GetAsync(
                    $"{Endpoint}/producto/{idProducto}");

            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<InventarioSalida>();
        }

        public async Task<InventarioSalida?> CrearAsync(
            InventarioGuardar dto)
        {
            using var respuesta =
                await _httpClient.PostAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<InventarioSalida>();
        }

        public async Task<InventarioSalida?> ModificarAsync(
            InventarioModificar dto)
        {
            using var respuesta =
                await _httpClient.PutAsJsonAsync(Endpoint, dto);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<InventarioSalida>();
        }
    }
}