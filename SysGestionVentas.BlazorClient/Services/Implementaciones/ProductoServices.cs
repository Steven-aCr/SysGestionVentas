using SysGestionVentas.BlazorClient.DTOs;
using SysGestionVentas.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;
using static SysGestionVentas.BlazorClient.Pages.Producto;

namespace SysGestionVentas.BlazorClient.Services.Implementations
{
    public class ProductoServices : IProductoServices
    {
        private readonly HttpClient _http;

        public ProductoServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductoDto>?> GetProductosAsync()
        {
            return await _http.GetFromJsonAsync<List<ProductoDto>>("api/productos");
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(long id)
        {
            return await _http.GetFromJsonAsync<ProductoDto>($"api/productos/{id}");
        }

        public async Task<bool> CrearProductoAsync(ProductoDto producto)
        {
            var res = await _http.PostAsJsonAsync("api/productos", producto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarProductoAsync(long id)
        {
            var res = await _http.DeleteAsync($"api/productos/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}