using System;
using System.Collections.Generic;
using System.Text;
using SysGestionVentas.ApiClient.DTOs.Inventario;

namespace SysGestionVentas.ApiClient.Services.Interfaces
{
    public interface IInventarioApiService
    {
        Task<List<InventarioSalida>> ObtenerTodosAsync();
        Task<InventarioSalida?> ObtenerPorIdAsync(int id);
        Task<bool> CrearAsync(InventarioGuardar dto);
        Task<bool> ModificarAsync(InventarioModificar dto);
        Task<bool> EliminarAsync(int id);
    }
}
