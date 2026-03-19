using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace BDGestionVentas.DAL
{
    public class ClientDAL
    {
        public static async Task<int> GuardarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pClient);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var client = await dbContexto.Client.FirstOrDefaultAsync(
                        c => c.ClientId == pClient.ClientId);

                    client.PersonId = pClient.PersonId;

                    dbContexto.Update(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Client pClient)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var client = await dbContexto.Client.FirstOrDefaultAsync(
                        c => c.ClientId == pClient.ClientId);

                    dbContexto.Remove(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<Client> ObtenerPorIdAsync(Client pClient)
        {
            var result = new Client();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Client
                        .Include(c => c.Person)
                        .FirstOrDefaultAsync(c => c.ClientId == pClient.ClientId);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<List<Client>> ObtenerTodosAsync(Client pClient)
        {
            var result = new List<Client>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Client
                        .Include(c => c.Person)
                        .Where(c =>
                            (pClient.PersonId == 0 || c.PersonId == pClient.PersonId)
                        )
                        .OrderBy(c => c.Person.LastName)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}