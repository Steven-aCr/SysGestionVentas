using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Status"/>.
    /// Orquesta validaciones de datos y delega la persistencia a <see cref="StatusDAL"/>.
    /// </summary>
    public class StatusBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Status"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Status pStatus)
        {
            var contexto = new ValidationContext(pStatus);
            var resultados = new List<ValidationResult>();
            bool esValido = Validator.TryValidateObject(pStatus, contexto, resultados, validateAllProperties: true);
            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo estado en el sistema.
        /// Valida unicidad del <c>Name</c> dentro del mismo <c>StatusTypeId</c> en la capa DAL.
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el nombre ya existe en el tipo o si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Status pStatus)
        {
            ValidarEntidad(pStatus);
            return await StatusDAL.GuardarAsync(pStatus);
        }

        /// <summary>
        /// Valida y modifica los datos de un estado existente en el sistema.
        /// Valida unicidad del <c>Name</c> dentro del mismo <c>StatusTypeId</c> en la capa DAL.
        /// </summary>
        /// <param name="pStatus">
        /// Objeto <see cref="Status"/> con el <c>StatusId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el estado no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Status pStatus)
        {
            ValidarEntidad(pStatus);
            return await StatusDAL.ModificarAsync(pStatus);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un estado, desactivándolo en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pStatus">
        /// Objeto <see cref="Status"/> con el <c>StatusId</c> del registro a desactivar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, si el estado no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Status pStatus)
        {
            if (pStatus.StatusId <= 0)
                throw new Exception("El ID de estado no es válido.");

            return await StatusDAL.EliminarAsync(pStatus);
        }

        /// <summary>
        /// Obtiene un estado específico por su identificador, incluyendo su relación con <see cref="StatusType"/>.
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> con el <c>StatusId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Status"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Status?> ObtenerPorIdAsync(Status pStatus)
        {
            if (pStatus.StatusId <= 0)
                throw new Exception("El ID de estado no es válido.");

            return await StatusDAL.ObtenerPorIdAsync(pStatus);
        }

        /// <summary>
        /// Obtiene una lista de estados aplicando filtros opcionales.
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> usado como filtro de búsqueda.</param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>Lista de objetos <see cref="Status"/> ordenados por tipo y nombre.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Status>> ObtenerTodosAsync(Status pStatus, bool? pIsActive = null)
        {
            return await StatusDAL.ObtenerTodosAsync(pStatus, pIsActive);
        }

        /// <summary>
        /// Obtiene una lista de estados filtrada por una colección de <c>StatusTypeId</c>.
        /// </summary>
        /// <param name="pStatusTypes">Lista de identificadores de tipos de estado a incluir.</param>
        /// <param name="pIsActive">Filtro de estado activo/inactivo. <c>null</c> devuelve todos.</param>
        /// <returns>Lista de objetos <see cref="Status"/> que corresponden a los tipos indicados.</returns>
        /// <exception cref="Exception">Se lanza si la lista es nula/vacía o si ocurre un error en base de datos.</exception>
        public static async Task<List<Status>> ObtenerPorTiposAsync(List<int> pStatusTypes, bool? pIsActive = null)
        {
            if (pStatusTypes == null || pStatusTypes.Count == 0)
                throw new Exception("Debe indicar al menos un tipo de estado.");

            return await StatusDAL.ObtenerPorTiposAsync(pStatusTypes, pIsActive);
        }

        #endregion
    }
}