using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using SysGestionVentas.ApiClient.DTOs.Inventario;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Services.Implementaciones
{
    public class InventarioApiService : IInventarioApiService
    {
        private readonly HttpClient _httpClient;

        public InventarioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<InventarioSalida>> ObtenerTodosAsync()
        {
            var respuesta = await _httpClient.GetFromJsonAsync<List<InventarioSalida>>("api/inventario");
            return respuesta ?? new List<InventarioSalida>();
        }

        public async Task<InventarioSalida?> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<InventarioSalida>($"api/inventario/{id}");
        }

        public async Task<bool> CrearAsync(InventarioGuardar dto)
        {
            var respuesta = await _httpClient.PostAsJsonAsync("api/inventario", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> ModificarAsync(InventarioModificar dto)
        {
            var respuesta = await _httpClient.PutAsJsonAsync($"api/inventario/{dto.Id}", dto);
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var respuesta = await _httpClient.DeleteAsync($"api/inventario/{id}");
            return respuesta.IsSuccessStatusCode;
        }
    }
}