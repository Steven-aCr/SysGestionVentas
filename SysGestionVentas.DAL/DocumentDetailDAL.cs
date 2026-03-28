using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DocumentDetailDAL
    {
        #region "CRUD"

        /// <summary>
        /// Registra un nuevo detalle de documento en la base de datos
        /// creando su propio contexto internamente.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con los datos del detalle a guardar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(DocumentDetail pDocumentDetail)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pDocumentDetail);
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
        /// Modifica los datos de un detalle de documento existente en la base de datos.
        /// Solo se permiten modificaciones en documentos que aún no hayan sido emitidos o cerrados;
        /// esta validación debe ser garantizada por la capa de negocio antes de invocar este método.
        /// Los campos <c>DocumentId</c> y <c>ProductId</c> no son modificables tras la creación.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a modificar
        /// y los nuevos valores calculados a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el detalle no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(DocumentDetail pDocumentDetail)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var detail = await dbContexto.DocumentDetail.FirstOrDefaultAsync(
                        dd => dd.DocDetailId == pDocumentDetail.DocDetailId);

                    if (detail == null)
                        throw new Exception($"No se encontró el detalle con ID {pDocumentDetail.DocDetailId}.");

                    detail.Quantity = pDocumentDetail.Quantity;
                    detail.UnitPrice = pDocumentDetail.UnitPrice;
                    detail.DiscountAmount = pDocumentDetail.DiscountAmount;
                    detail.Subtotal = pDocumentDetail.Subtotal;
                    detail.TaxPercentage = pDocumentDetail.TaxPercentage;
                    detail.TaxAmount = pDocumentDetail.TaxAmount;
                    detail.TotalAmount = pDocumentDetail.TotalAmount;
                    detail.Notes = pDocumentDetail.Notes;

                    dbContexto.Update(detail);
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
        /// Elimina físicamente un detalle de documento de la base de datos.
        /// A diferencia de otras entidades del sistema, <see cref="DocumentDetail"/> no posee
        /// <c>StatusId</c> propio, por lo que la eliminación es física y restringida
        /// a documentos en estado editable (borrador/pendiente).
        /// La validación del estado del documento padre debe ser responsabilidad
        /// de la capa de negocio antes de invocar este método.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se eliminó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el detalle no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(DocumentDetail pDocumentDetail)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var detail = await dbContexto.DocumentDetail.FirstOrDefaultAsync(
                        dd => dd.DocDetailId == pDocumentDetail.DocDetailId);

                    if (detail == null)
                        throw new Exception($"No se encontró el detalle con ID {pDocumentDetail.DocDetailId}.");

                    // DocumentDetail no tiene StatusId propio. La eliminación es física
                    // pero está restringida por diseño a documentos en estado editable.
                    dbContexto.DocumentDetail.Remove(detail);
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
        /// Obtiene un detalle de documento específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Document"/> y <see cref="ProductList"/>.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> a buscar.
        /// </param>
        /// <returns>
        /// El objeto <see cref="DocumentDetail"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<DocumentDetail?> ObtenerPorIdAsync(DocumentDetail pDocumentDetail)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.DocumentDetail
                        .Include(dd => dd.Document)
                        .Include(dd => dd.Product)
                        .FirstOrDefaultAsync(dd => dd.DocDetailId == pDocumentDetail.DocDetailId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los detalles asociados a un documento específico,
        /// incluyendo la información del producto en cada línea.
        /// </summary>
        /// <param name="pDocumentId">Identificador del documento cuyas líneas de detalle se desean consultar.</param>
        /// <returns>
        /// Lista de objetos <see cref="DocumentDetail"/> del documento indicado,
        /// con la navegación a <see cref="ProductList"/> cargada.
        /// Retorna lista vacía si el documento no tiene detalles registrados.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<DocumentDetail>> ObtenerPorDocumentoAsync(int pDocumentId)
        {
            var result = new List<DocumentDetail>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.DocumentDetail
                        .Include(dd => dd.Product)
                        .Where(dd => dd.DocumentId == pDocumentId)
                        .OrderBy(dd => dd.DocDetailId)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de detalles de documentos aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>DocumentId</c>: filtra por documento asociado (0 = sin filtro).</description></item>
        ///   <item><description><c>ProductId</c>: filtra por producto (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="DocumentDetail"/> que cumplen los filtros indicados,
        /// ordenados por <c>DocumentId</c> y luego por <c>DocDetailId</c> de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<DocumentDetail>> ObtenerTodosAsync(DocumentDetail pDocumentDetail)
        {
            var result = new List<DocumentDetail>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.DocumentDetail
                        .Include(dd => dd.Document)
                        .Include(dd => dd.Product)
                        .Where(dd =>
                            (pDocumentDetail.DocumentId == 0 || dd.DocumentId == pDocumentDetail.DocumentId) &&
                            (pDocumentDetail.ProductId == 0 || dd.ProductId == pDocumentDetail.ProductId)
                        )
                        .OrderBy(dd => dd.DocumentId)
                            .ThenBy(dd => dd.DocDetailId)
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
    }
}