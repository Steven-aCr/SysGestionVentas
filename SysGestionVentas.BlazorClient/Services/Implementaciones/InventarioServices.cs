using SysGestionVentas.BlazorClient.DTOs;
using SysGestionVentas.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;
using static SysGestionVentas.BlazorClient.Pages.Inventario;

namespace SysGestionVentas.BlazorClient.Services.Implementations
{
    public class InventarioServices : IInventarioServices
    {
        private readonly HttpClient _http;

        public InventarioServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<InventarioDto>?> GetInventarioAsync()
        {
            return await _http.GetFromJsonAsync<List<InventarioDto>>("api/inventario");
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