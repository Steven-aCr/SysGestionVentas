using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DocumentTypeDAL
    {
        /// <summary>
        /// Verifica si ya existe un tipo de documento con el mismo nombre.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos.</param>
        /// <returns>
        /// <c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.
        /// </returns>
        private static async Task<bool> ExisteNombre(DocumentType pDocType, DbContexto pDBContexto)
        {
            var docTypeExiste = await pDBContexto.DocumentType.FirstOrDefaultAsync(
                d => d.Name == pDocType.Name && d.DocTypeId != pDocType.DocTypeId);

            return (docTypeExiste != null && docTypeExiste.DocTypeId > 0);
        }

        /// <summary>
        /// Registra un nuevo tipo de documento en la base de datos.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(DocumentType pDocType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pDocType, dbContexto))
                        throw new Exception("El tipo de documento ya existe.");

                    dbContexto.Add(pDocType);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Modifica un tipo de documento existente en la base de datos.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el registro no existe, si el nombre está duplicado o si ocurre un error.
        /// </exception>
        public static async Task<int> ModificarAsync(DocumentType pDocType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pDocType, dbContexto))
                        throw new Exception("El tipo de documento ya existe.");

                    var docType = await dbContexto.DocumentType.FirstOrDefaultAsync(
                        d => d.DocTypeId == pDocType.DocTypeId);

                    if (docType == null)
                        throw new Exception($"No se encontró el tipo de documento con ID {pDocType.DocTypeId}.");

                    docType.Name = pDocType.Name;
                    docType.Description = pDocType.Description;

                    dbContexto.Update(docType);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Realiza una eliminación lógica de un tipo de documento.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el registro no existe o ocurre un error.
        /// </exception>
        public static async Task<int> EliminarAsync(DocumentType pDocType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var docType = await dbContexto.DocumentType.FirstOrDefaultAsync(
                        d => d.DocTypeId == pDocType.DocTypeId);

                    if (docType == null)
                        throw new Exception($"No se encontró el tipo de documento con ID {pDocType.DocTypeId}.");

                    // Eliminación lógica
                    docType.StatusId = pDocType.StatusId;

                    dbContexto.Update(docType);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene un tipo de documento por su ID.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con el ID a buscar.</param>
        /// <returns>
        /// El tipo de documento encontrado o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<DocumentType> ObtenerPorIdAsync(DocumentType pDocType)
        {
            var result = new DocumentType();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.DocumentType
                        .FirstOrDefaultAsync(d => d.DocTypeId == pDocType.DocTypeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de tipos de documento aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> usado como filtro:
        /// <list type="bullet">
        /// <item><description><c>Name</c>: filtro por nombre (opcional).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de tipos de documento que cumplen los filtros.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error.</exception>
        public static async Task<List<DocumentType>> ObtenerTodosAsync(DocumentType pDocType)
        {
            var result = new List<DocumentType>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.DocumentType
                        .Where(d =>
                            (pDocType.Name == null || d.Name.Contains(pDocType.Name))
                        )
                        .OrderBy(d => d.Name)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}