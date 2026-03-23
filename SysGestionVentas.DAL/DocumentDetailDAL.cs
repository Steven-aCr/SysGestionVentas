using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class DocumentDetailDAL
    {
        /// <summary>
        /// Método para guardar información del detalle de documento
        /// </summary>
        /// <param name="pDocumentDetail">Entidad que contiene los datos del detalle a guardar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Guarda los datos del detalle de documento y retorna el número de filas afectadas</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar guardar</exception>
        public static async Task<int> GuardarAsync(DocumentDetail pDocumentDetail, DbContexto dbContexto)
        {
            try
            {
                // Se especifica el DbSet (DocumentDetail) al que se agregará la entidad
                dbContexto.DocumentDetail.Add(pDocumentDetail);

                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar el detalle del documento.", ex);
            }
        }
    }
}