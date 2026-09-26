using System.Net.Http.Json;
using SysGestionVentas.BlazorClient.DTOs;
using SysGestionVentas.BlazorClient.Services.Interfaces;

namespace SysGestionVentas.BlazorClient.Services.Implementations
{
    public class MovimientoInventarioServices : IMovimientoInventarioServices
    {
        private readonly HttpClient _http;

        public MovimientoInventarioServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MovimientoInventarioDto>?> GetMovimientosAsync()
        {
            return await _http.GetFromJsonAsync<List<MovimientoInventarioDto>>("api/movimientos-inventario");
        }

        public async Task<bool> RegistrarMovimientoAsync(MovimientoInventarioDto movimiento)
        {
            var res = await _http.PostAsJsonAsync("api/movimientos-inventario", movimiento);
            return res.IsSuccessStatusCode;
        }
    }
}