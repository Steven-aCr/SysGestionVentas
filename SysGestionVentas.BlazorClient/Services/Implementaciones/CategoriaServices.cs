using SysGestionVentas.BlazorClient.DTOs;
using SysGestionVentas.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;
using static SysGestionVentas.BlazorClient.Pages.Categoria;

namespace SysGestionVentas.BlazorClient.Services.Implementations
{
    public class CategoriaServices : ICategoriaServices
    {
        private readonly HttpClient _http;

        public CategoriaServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CategoriaDto>?> GetCategoriasAsync()
        {
            return await _http.GetFromJsonAsync<List<CategoriaDto>>("api/categorias");
        }

        public async Task<bool> CrearCategoriaAsync(CategoriaDto categoria)
        {
            var res = await _http.PostAsJsonAsync("api/categorias", categoria);
            return res.IsSuccessStatusCode;
        }
    }
}