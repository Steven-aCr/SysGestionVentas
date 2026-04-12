using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class ClientBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Client"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Client pClient)
        {
            var contexto = new ValidationContext(pClient);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pClient, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo cliente en el sistema.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Client pClient)
        {
            ValidarEntidad(pClient);
            return await ClientDAL.GuardarAsync(pClient);
        }

        /// <summary>
        /// Valida y modifica los datos de un cliente existente en el sistema.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con el <c>ClientId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el cliente no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Client pClient)
        {
            if (pClient.ClientId <= 0)
                throw new Exception("El ID de cliente no es válido.");

            ValidarEntidad(pClient);
            return await ClientDAL.ModificarAsync(pClient);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un cliente cambiando el estado
        /// de su <see cref="Person"/> asociada. No elimina el registro físicamente.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con el <c>ClientId</c> del registro a desactivar
        /// y la <see cref="Person"/> con el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el cliente no existe, o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Client pClient)
        {
            if (pClient.ClientId <= 0)
                throw new Exception("El ID de cliente no es válido.");

            if (pClient.Person == null || pClient.Person.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await ClientDAL.EliminarAsync(pClient);
        }

        /// <summary>
        /// Obtiene un cliente específico por su identificador, incluyendo
        /// su relación con <see cref="Person"/> y el <see cref="Status"/> de la persona.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> con el <c>ClientId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Client"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Client?> ObtenerPorIdAsync(Client pClient)
        {
            if (pClient.ClientId <= 0)
                throw new Exception("El ID de cliente no es válido.");

            return await ClientDAL.ObtenerPorIdAsync(pClient);
        }

        /// <summary>
        /// Obtiene una lista de clientes aplicando filtros opcionales.
        /// </summary>
        /// <param name="pClient">Objeto <see cref="Client"/> usado como filtro de búsqueda.</param>
        /// <returns>
        /// Lista de objetos <see cref="Client"/> ordenados por apellido
        /// y nombre de la persona asociada de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Client>> ObtenerTodosAsync(Client pClient)
        {
            return await ClientDAL.ObtenerTodosAsync(pClient);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de clientes con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Client}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Client}"/> con la lista de clientes encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<PagedResult<Client>> BuscarAsync(PagedQuery<Client> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await ClientDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}