using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class DocumentBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Document"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación definidas en la entidad.
        /// El mensaje de la excepción contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Document pDocument)
        {
            var contexto = new ValidationContext(pDocument);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pDocument, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo documento en el sistema.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con los datos del documento a registrar.
        /// Los campos <c>DocTypeId</c>, <c>DocNumber</c>, <c>IssueDate</c> y <c>PersonId</c>
        /// son obligatorios.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación en base de datos.</exception>
        public static async Task<int> GuardarAsync(Document pDocument)
        {
            ValidarEntidad(pDocument);
            return await DocumentDAL.GuardarAsync(pDocument);
        }

        /// <summary>
        /// Valida y modifica los datos de un documento existente en el sistema.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el documento no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Document pDocument)
        {
            ValidarEntidad(pDocument);
            return await DocumentDAL.ModificarAsync(pDocument);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un documento, cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            if (pDocument.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await DocumentDAL.EliminarAsync(pDocument);
        }

        /// <summary>
        /// Obtiene un documento específico por su identificador, incluyendo
        /// sus relaciones con <see cref="DocumentType"/> y <see cref="Person"/>.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con el <c>DocumentId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Document"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Document?> ObtenerPorIdAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            return await DocumentDAL.ObtenerPorIdAsync(pDocument);
        }

        /// <summary>
        /// Obtiene una lista de documentos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>DocNumber</c>: filtra por coincidencia parcial en el número (null = sin filtro).</description></item>
        ///   <item><description><c>DocTypeId</c>: filtra por tipo de documento (0 = sin filtro).</description></item>
        ///   <item><description><c>PersonId</c>: filtra por persona asociada (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Document"/> que cumplen los filtros indicados,
        /// ordenados por fecha de emisión de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Document>> ObtenerTodosAsync(Document pDocument)
        {
            return await DocumentDAL.ObtenerTodosAsync(pDocument);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de documentos con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Document}"/> que define los filtros, el tamaño de página
        /// y el número de página. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Document}"/> con la lista de documentos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Document>> BuscarAsync(PagedQuery<Document> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await DocumentDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}