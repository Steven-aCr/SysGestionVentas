using SysGestionVentas.BlazorClient.DTOs;
using static SysGestionVentas.BlazorClient.Pages.Inventario;

namespace SysGestionVentas.BlazorClient.Services.Interfaces
{
    public interface IInventarioServices
    {
        Task<List<InventarioDto>?> GetInventarioAsync();
        Task<List<MovimientoInventarioDto>?> GetMovimientosAsync();
        Task<bool> RegistrarMovimientoAsync(MovimientoInventarioDto movimiento);
    }
}