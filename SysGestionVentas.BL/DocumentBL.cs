using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Document"/>.
    /// Orquesta validaciones, reglas del ciclo de vida documental y delega
    /// la persistencia a <see cref="DocumentDAL"/>.
    /// </summary>
    public class DocumentBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Document"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
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
        /// <param name="pDocument">Objeto <see cref="Document"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Document pDocument)
        {
            ValidarEntidad(pDocument);
            return await DocumentDAL.GuardarAsync(pDocument);
        }

        /// <summary>
        /// Valida y modifica los datos editables de un documento existente.
        /// Los campos <c>DocNumber</c>, <c>DocTypeId</c> y <c>CreatedByUser</c>
        /// no son modificables tras la emisión del documento.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el documento no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            ValidarEntidad(pDocument);
            return await DocumentDAL.ModificarAsync(pDocument);
        }

        /// <summary>
        /// Realiza la eliminación lógica (anulación) de un documento, cambiando su estado.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado "Anulado".
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se anuló correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, el documento no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            if (pDocument.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la anulación del documento.");

            return await DocumentDAL.EliminarAsync(pDocument);
        }

        /// <summary>
        /// Obtiene un documento específico por su identificador, incluyendo sus relaciones
        /// con <see cref="DocumentType"/>, <see cref="Person"/>, <see cref="Status"/>
        /// y el <see cref="User"/> que lo creó.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con el <c>DocumentId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Document"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Document?> ObtenerPorIdAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            return await DocumentDAL.ObtenerPorIdAsync(pDocument);
        }

        /// <summary>
        /// Obtiene una lista de documentos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Document"/> ordenados por fecha de emisión descendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Document>> ObtenerTodosAsync(Document pDocument)
        {
            return await DocumentDAL.ObtenerTodosAsync(pDocument);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de documentos con soporte para paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Document}"/> con los filtros y parámetros de paginación.
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