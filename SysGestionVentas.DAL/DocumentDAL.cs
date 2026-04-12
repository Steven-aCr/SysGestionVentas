using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.DAL
{
    public class DocumentDAL
    {
        #region "Métodos Privados"

        /// Método privado para QuerySelect:
        private static IQueryable<Document> QuerySelect(
            IQueryable<Document> pQuery,
            PagedQuery<Document> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (!string.IsNullOrEmpty(f.DocNumber))
                pQuery = pQuery.Where(d => d.DocNumber!.Contains(f.DocNumber));

            if (f.DocTypeId > 0)
                pQuery = pQuery.Where(d => d.DocTypeId == f.DocTypeId);

            if (f.PersonId > 0)
                pQuery = pQuery.Where(d => d.PersonId == f.PersonId);

            if (f.StatusId > 0)
                pQuery = pQuery.Where(d => d.StatusId == f.StatusId);

            if (pPagedQuery.FromDate.HasValue)
                pQuery = pQuery.Where(d => d.IssueDate >= pPagedQuery.FromDate.Value);

            if (pPagedQuery.ToDate.HasValue)
                pQuery = pQuery.Where(d => d.IssueDate <= pPagedQuery.ToDate.Value);

            return pQuery.OrderByDescending(d => d.IssueDate);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo documento en la base de datos.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pDocument);
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
        /// Modifica los datos de un documento existente en la base de datos.
        /// Los campos estructurales <c>DocNumber</c>, <c>DocTypeId</c> y <c>CreatedByUser</c>
        /// no son modificables tras la emisión del documento.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el documento no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var document = await dbContexto.Document.FirstOrDefaultAsync(
                        d => d.DocumentId == pDocument.DocumentId);

                    if (document == null)
                        throw new Exception($"No se encontró el documento con ID {pDocument.DocumentId}.");

                    document.IssueDate = pDocument.IssueDate;
                    document.PersonId = pDocument.PersonId;
                    document.TotalAmount = pDocument.TotalAmount;
                    document.StatusId = pDocument.StatusId;

                    dbContexto.Update(document);
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
        /// Realiza una eliminación lógica de un documento, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo o anulado.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el documento no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var document = await dbContexto.Document.FirstOrDefaultAsync(
                        d => d.DocumentId == pDocument.DocumentId);

                    if (document == null)
                        throw new Exception($"No se encontró el documento con ID {pDocument.DocumentId}.");

                    // Eliminación lógica: se cambia el estado del documento
                    // en lugar de eliminarlo físicamente de la base de datos.
                    document.StatusId = pDocument.StatusId;

                    dbContexto.Update(document);
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
        /// Obtiene un documento específico por su identificador, incluyendo sus relaciones
        /// con <see cref="DocumentType"/>, <see cref="Person"/>, <see cref="Status"/>
        /// y el <see cref="User"/> que lo creó.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con el <c>DocumentId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Document"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Document?> ObtenerPorIdAsync(Document pDocument)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Document
                        .Include(d => d.DocumentType)
                        .Include(d => d.Person)
                        .Include(d => d.Status)
                        .Include(d => d.CreatedBy)
                        .FirstOrDefaultAsync(d => d.DocumentId == pDocument.DocumentId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de documentos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>DocNumber</c>: filtra por coincidencia parcial en el número de documento (null = sin filtro).</description></item>
        ///   <item><description><c>DocTypeId</c>: filtra por tipo de documento (0 = sin filtro).</description></item>
        ///   <item><description><c>PersonId</c>: filtra por persona asociada (0 = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado del documento (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Document"/> que cumplen los filtros indicados,
        /// ordenados por fecha de emisión de forma descendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Document>> ObtenerTodosAsync(Document pDocument)
        {
            var result = new List<Document>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Document
                        .Include(d => d.DocumentType)
                        .Include(d => d.Person)
                        .Include(d => d.Status)
                        .Include(d => d.CreatedBy)
                        .Where(d =>
                            (pDocument.DocNumber == null || d.DocNumber!.Contains(pDocument.DocNumber)) &&
                            (pDocument.DocTypeId == 0 || d.DocTypeId == pDocument.DocTypeId) &&
                            (pDocument.PersonId == 0 || d.PersonId == pDocument.PersonId) &&
                            (pDocument.StatusId == 0 || d.StatusId == pDocument.StatusId)
                        )
                        .OrderByDescending(d => d.IssueDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        #endregion

        #region "Search"

        /// Método público para búsqueda con filtros y paginación:
        public static async Task<PagedResult<Document>> BuscarAsync(
            PagedQuery<Document> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Document
                        .Include(d => d.DocumentType)
                        .Include(d => d.Person)
                        .Include(d => d.Status)
                        .Include(d => d.CreatedBy)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);

                    int total = await filtered.CountAsync();

                    List<Document> items;
                    if (pPagedQuery.Top > 0)
                    {
                        items = await filtered
                            .Take(pPagedQuery.Top)
                            .ToListAsync();
                    }
                    else
                    {
                        items = await filtered
                            .Skip(pPagedQuery.Skip)
                            .Take(pPagedQuery.PageSize)
                            .ToListAsync();
                    }

                    return new PagedResult<Document>
                    {
                        Items = items,
                        TotalCount = total,
                        CurrentPage = pPagedQuery.Page,
                        PageSize = pPagedQuery.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}