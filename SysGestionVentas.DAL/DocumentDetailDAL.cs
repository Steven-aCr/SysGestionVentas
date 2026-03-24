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
                // Se especifica el DbSet al que se agregará la entidad
                dbContexto.DocumentDetail.Add(pDocumentDetail);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar el detalle del documento.", ex);
            }
        }

        /// <summary>
        /// Método para obtener todos los detalles asociados a un documento específico
        /// </summary>
        /// <param name="idDocumento">ID del documento padre</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna una lista con los detalles pertenecientes al documento</returns>
        public static async Task<List<DocumentDetail>> ObtenerPorDocumentoAsync(int idDocumento, DbContexto dbContexto)
        {
            return await dbContexto.DocumentDetail
                .Where(d => d.IdDocument == idDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Método para obtener un detalle de documento específico por su ID
        /// </summary>
        /// <param name="id">El identificador único del detalle</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna la entidad del detalle si la encuentra, o null si no existe</returns>
        public static async Task<DocumentDetail> ObtenerPorIdAsync(int id, DbContexto dbContexto)
        {
            return await dbContexto.DocumentDetail.FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Método para modificar la información de un detalle de documento existente
        /// </summary>
        /// <param name="pDocumentDetail">Entidad que contiene los datos actualizados</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar modificar</exception>
        public static async Task<int> ModificarAsync(DocumentDetail pDocumentDetail, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que actualice la entidad
                dbContexto.DocumentDetail.Update(pDocumentDetail);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al modificar el detalle del documento.", ex);
            }
        }

        /// <summary>
        /// Método para eliminar un detalle de documento de la base de datos
        /// </summary>
        /// <param name="pDocumentDetail">Entidad del detalle que se desea eliminar</param>
        /// <param name="dbContexto">Contexto de la base de datos</param>
        /// <returns>Retorna el número de filas afectadas en la base de datos</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error al intentar eliminar</exception>
        public static async Task<int> EliminarAsync(DocumentDetail pDocumentDetail, DbContexto dbContexto)
        {
            try
            {
                // Se indica a Entity Framework que elimine la entidad
                dbContexto.DocumentDetail.Remove(pDocumentDetail);
                return await dbContexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el detalle del documento.", ex);
            }
        }
    }
}