using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class DocumentTypeBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="DocumentType"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(DocumentType pDocType)
        {
            var contexto = new ValidationContext(pDocType);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pDocType, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo tipo de documento en el sistema.
        /// Verifica unicidad del nombre en la capa DAL.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(DocumentType pDocType)
        {
            ValidarEntidad(pDocType);
            return await DocumentTypeDAL.GuardarAsync(pDocType);
        }

        /// <summary>
        /// Valida y modifica los datos de un tipo de documento existente en el sistema.
        /// Verifica unicidad del nombre en la capa DAL.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el registro no existe, si el nombre está duplicado,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(DocumentType pDocType)
        {
            if (pDocType.DocTypeId <= 0)
                throw new Exception("El ID de tipo de documento no es válido.");

            ValidarEntidad(pDocType);
            return await DocumentTypeDAL.ModificarAsync(pDocType);
        }

        /// <summary>
        /// Obtiene un tipo de documento específico por su identificador.
        /// </summary>
        /// <remarks>
        /// <see cref="DocumentType"/> no implementa eliminación lógica propia dado que su estado
        /// se controla mediante el campo <c>StatusId</c> gestionado directamente en el DAL.
        /// Para desactivar un tipo de documento, utilice <see cref="ModificarAsync"/> ajustando
        /// el <c>StatusId</c> al estado inactivo correspondiente.
        /// </remarks>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> con el <c>DocTypeId</c> a buscar.</param>
        /// <returns>El objeto <see cref="DocumentType"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<DocumentType?> ObtenerPorIdAsync(DocumentType pDocType)
        {
            if (pDocType.DocTypeId <= 0)
                throw new Exception("El ID de tipo de documento no es válido.");

            return await DocumentTypeDAL.ObtenerPorIdAsync(pDocType);
        }

        /// <summary>
        /// Obtiene una lista de tipos de documento aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDocType">Objeto <see cref="DocumentType"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="DocumentType"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<DocumentType>> ObtenerTodosAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.ObtenerTodosAsync(pDocType);
        }

        #endregion
    }
}