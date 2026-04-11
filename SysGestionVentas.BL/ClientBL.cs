using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.BL
{
    public class ClientBL
    {
        public async Task<int> GuardarAsync(Client pClient)
        {
            if (pClient == null) return 0;
            return await ClientDAL.GuardarAsync(pClient);
        }

        public async Task<int> ModificarAsync(Client pClient)
        {
            if (pClient == null || pClient.ClientId <= 0) return 0;
            return await ClientDAL.ModificarAsync(pClient);
        }

        public async Task<int> EliminarAsync(int id)
        {
            if (id <= 0) return 0;
            return await ClientDAL.EliminarAsync(id);
        }

        public async Task<List<Client>> ObtenerTodosAsync()
        {
            return await ClientDAL.ObtenerTodosAsync();
        }

        public async Task<Client?> ObtenerPorIdAsync(int id)
        {
            if (id <= 0) return null;
            return await ClientDAL.ObtenerPorIdAsync(id);
        }
    }
}