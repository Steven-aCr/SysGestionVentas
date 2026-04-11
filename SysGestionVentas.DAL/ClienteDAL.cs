using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class ClientDAL
    {
        // Guardar un nuevo registro
        public static async Task<int> GuardarAsync(Client pClient)
        {
            int result = 0;
            using (var dbContexto = new DbContexto())
            {
                dbContexto.Client.Add(pClient);
                result = await dbContexto.SaveChangesAsync();
            }
            return result;
        }

        // Modificar un registro existente
        public static async Task<int> ModificarAsync(Client pClient)
        {
            int result = 0;
            using (var dbContexto = new DbContexto())
            {
                var client = await dbContexto.Client.FirstOrDefaultAsync(c => c.ClientId == pClient.ClientId);
                if (client != null)
                {
                    // Actualización de campos
                    client.PersonId = pClient.PersonId;
                    client.StatusId = pClient.StatusId;
                    client.Address = pClient.Address;
                    client.Name = pClient.Name;
                    client.NumberPhone = pClient.NumberPhone;
                    client.DocumentTypeId = pClient.DocumentTypeId;

                    dbContexto.Update(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            return result;
        }

        // Eliminar un registro por ID
        public static async Task<int> EliminarAsync(int pId)
        {
            int result = 0;
            using (var dbContexto = new DbContexto())
            {
                var client = await dbContexto.Client.FirstOrDefaultAsync(c => c.ClientId == pId);
                if (client != null)
                {
                    dbContexto.Client.Remove(client);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            return result;
        }

        // Obtener un solo registro por ID (incluyendo relaciones)
        public static async Task<Client?> ObtenerPorIdAsync(int pId)
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Client
                    .Include(c => c.Person)
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.ClientId == pId);
            }
        }

        // Obtener la lista completa
        public static async Task<List<Client>> ObtenerTodosAsync()
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Client
                    .Include(c => c.Person)
                    .Include(c => c.Status)
                    .ToListAsync();
            }
        }
    }
}