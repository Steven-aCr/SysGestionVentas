using SysGestionVentas.BlazorClient.DTOs;

namespace SysGestionVentas.BlazorClient.Services.Interfaces
{
    public interface IMovimientoInventarioServices
    {
        Task<List<MovimientoInventarioDto>?> GetMovimientosAsync();
        Task<bool> RegistrarMovimientoAsync(MovimientoInventarioDto movimiento);
    }
}