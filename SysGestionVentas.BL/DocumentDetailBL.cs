using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class DocumentDetailBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="DocumentDetail"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDocumentDetail">Objeto <see cref="DocumentDetail"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación definidas en la entidad.
        /// El mensaje de la excepción contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(DocumentDetail pDocumentDetail)
        {
            var contexto = new ValidationContext(pDocumentDetail);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pDocumentDetail, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo detalle de documento en el sistema.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con los datos del detalle a registrar.
        /// Los campos <c>DocumentId</c>, <c>ProductId</c> y <c>Quantity</c> son obligatorios.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación en base de datos.</exception>
        public static async Task<int> GuardarAsync(DocumentDetail pDocumentDetail)
        {
            ValidarEntidad(pDocumentDetail);
            return await DocumentDetailDAL.GuardarAsync(pDocumentDetail);
        }

        /// <summary>
        /// Valida y modifica los datos de un detalle de documento existente en el sistema.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el detalle no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(DocumentDetail pDocumentDetail)
        {
            ValidarEntidad(pDocumentDetail);
            return await DocumentDetailDAL.ModificarAsync(pDocumentDetail);
        }

        /// <summary>
        /// Elimina físicamente un detalle de documento de la base de datos.
        /// Esta operación es irreversible.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se eliminó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(DocumentDetail pDocumentDetail)
        {
            if (pDocumentDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle de documento no es válido.");

            return await DocumentDetailDAL.EliminarAsync(pDocumentDetail);
        }

        /// <summary>
        /// Obtiene un detalle de documento específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Document"/> y <see cref="ProductList"/>.
        /// </summary>
        /// <param name="pDocumentDetail">Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="DocumentDetail"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<DocumentDetail?> ObtenerPorIdAsync(DocumentDetail pDocumentDetail)
        {
            if (pDocumentDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle de documento no es válido.");

            return await DocumentDetailDAL.ObtenerPorIdAsync(pDocumentDetail);
        }

        /// <summary>
        /// Obtiene una lista de detalles de documento aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDocumentDetail">
        /// Objeto <see cref="DocumentDetail"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>DocumentId</c>: filtra por documento asociado (0 = sin filtro).</description></item>
        ///   <item><description><c>ProductId</c>: filtra por producto asociado (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="DocumentDetail"/> que cumplen los filtros indicados,
        /// ordenados por documento de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<DocumentDetail>> ObtenerTodosAsync(DocumentDetail pDocumentDetail)
        {
            return await DocumentDetailDAL.ObtenerTodosAsync(pDocumentDetail);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de detalles de documento con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{DocumentDetail}"/> que define los filtros, el tamaño de página
        /// y el número de página. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{DocumentDetail}"/> con la lista de detalles encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<DocumentDetail>> BuscarAsync(PagedQuery<DocumentDetail> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await DocumentDetailDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}