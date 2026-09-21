using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.MovimientoInventario;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class MovimientoInventarioApiService
        : IMovimientoInventarioApiService
    {
        private readonly HttpClient _httpClient;

        public MovimientoInventarioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MovimientoInventarioSalida>> ObtenerTodosAsync()
        {
            var respuesta = await _httpClient
                .GetFromJsonAsync<List<MovimientoInventarioSalida>>(
                    "api/movimientos-inventario");

            return respuesta ?? new List<MovimientoInventarioSalida>();
        }

        public async Task<MovimientoInventarioSalida?> ObtenerPorIdAsync(int id)
        {
            var respuesta = await _httpClient.GetAsync(
                $"api/movimientos-inventario/{id}");

            if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<MovimientoInventarioSalida>();
        }

        public async Task<List<MovimientoInventarioSalida>>
            ObtenerPorInventarioAsync(int idInventario)
        {
            var respuesta = await _httpClient
                .GetFromJsonAsync<List<MovimientoInventarioSalida>>(
                    $"api/movimientos-inventario/inventario/{idInventario}");

            return respuesta ?? new List<MovimientoInventarioSalida>();
        }

        public async Task<MovimientoInventarioSalida?> GuardarAsync(
            MovimientoInventarioGuardar movimiento)
        {
            // El cálculo del stock no se realiza aquí.
            // C# únicamente envía el movimiento y Java aplica la regla de negocio.
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/movimientos-inventario",
                movimiento);

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<MovimientoInventarioSalida>();
        }
    }
}