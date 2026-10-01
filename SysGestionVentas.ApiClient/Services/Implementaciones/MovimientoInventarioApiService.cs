using System.Net;
using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.MovimientoInventario;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class MovimientoInventarioApiService
        : IMovimientoInventarioApiService
    {
        private const string Endpoint = "api/movimientos-inventario";
        private readonly HttpClient _httpClient;

        public MovimientoInventarioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MovimientoInventarioSalida>> ObtenerTodosAsync()
        {
            using var respuesta = await _httpClient.GetAsync(Endpoint);
            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                   .ReadFromJsonAsync<List<MovimientoInventarioSalida>>()
                   ?? new List<MovimientoInventarioSalida>();
        }

        public async Task<MovimientoInventarioSalida?> ObtenerPorIdAsync(int id)
        {
            using var respuesta = 
                await _httpClient.GetAsync($"{Endpoint}/{id}");

            if(respuesta.StatusCode == HttpStatusCode.NotFound)
                return null;

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<MovimientoInventarioSalida>();
        }

        public async Task<List<MovimientoInventarioSalida>>
            ObtenerPorInventarioAsync(int IdInventario)
        {
            using var respuesta = await _httpClient.GetAsync(
                $"{Endpoint}/inventario/{IdInventario}");

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<List<MovimientoInventarioSalida>>()
                ?? new List<MovimientoInventarioSalida>();
        }

        public async Task<MovimientoInventarioSalida?> GuardarAsync(
            MovimientoInventarioGuardar movimiento)
        {
            using var respuesta = await 
                _httpClient.PostAsJsonAsync(Endpoint, movimiento);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<MovimientoInventarioSalida>();
        }
    }
}