using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DocumentTypeDAL
    {
        /// <summary>
        /// Verifica si ya existe un tipo de documento con el mismo nombre en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con el <c>Name</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(DocumentType pDocType, DbContexto pDBContexto)
        {
            return await pDBContexto.DocumentType.AnyAsync(
                d => d.Name == pDocType.Name && d.DocTypeId != pDocType.DocTypeId);
        }

        /// <summary>
        /// Registra un nuevo tipo de documento en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de guardar.
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
        /// Modifica los datos de un tipo de documento existente en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de actualizar.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el registro no existe, si el nombre está duplicado,
        /// o si ocurre un error durante la operación.
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
        /// Obtiene un tipo de documento específico por su identificador.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="DocumentType"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<DocumentType?> ObtenerPorIdAsync(DocumentType pDocType)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.DocumentType
                        .FirstOrDefaultAsync(d => d.DocTypeId == pDocType.DocTypeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de tipos de documento aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="DocumentType"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<DocumentType>> ObtenerTodosAsync(DocumentType pDocType)
        {
            var result = new List<DocumentType>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.DocumentType
                        .Where(d =>
                            (pDocType.Name == null || d.Name!.Contains(pDocType.Name))
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