using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class InventoryMovementDAL
    {
        /// <summary>
        /// Método para registrar un movimiento de inventario
        /// </summary>
        /// <param name="pInventoryMovement">Entidad que contiene los datos del movimiento a registrar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Registra el movimiento y retorna el número de filas afectadas</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar registrar</exception>
        public static async Task<int> RegistrarMovimientoAsync(InventoryMovement pInventoryMovement, DbContexto dbContexto)
        {
            try
            {
                // Se especifica el DbSet (InventoryMovement) al que se agregará la entidad
                dbContexto.InventoryMovement.Add(pInventoryMovement);

                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al registrar el movimiento de inventario.", ex);
            }
        }
    }
}