using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class PersonBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Person"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Person pPerson)
        {
            var contexto = new ValidationContext(pPerson);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pPerson, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra una nueva persona en el sistema.
        /// Valida unicidad de <c>Dui</c> y <c>PhoneNumber</c> en la capa DAL.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el DUI o teléfono ya existen, o si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Person pPerson)
        {
            ValidarEntidad(pPerson);
            return await PersonDAL.GuardarAsync(pPerson);
        }

        /// <summary>
        /// Valida y modifica los datos de una persona existente en el sistema.
        /// Valida unicidad de <c>Dui</c> y <c>PhoneNumber</c> en la capa DAL.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> con el <c>PersonId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si la persona no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Person pPerson)
        {
            ValidarEntidad(pPerson);
            return await PersonDAL.ModificarAsync(pPerson);
        }

        /// <summary>
        /// Realiza la eliminación lógica de una persona, cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> con el <c>PersonId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, si la persona no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Person pPerson)
        {
            if (pPerson.PersonId <= 0)
                throw new Exception("El ID de persona no es válido.");

            if (pPerson.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await PersonDAL.EliminarAsync(pPerson);
        }

        /// <summary>
        /// Obtiene una persona específica por su identificador, incluyendo
        /// su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con el <c>PersonId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Person"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Person?> ObtenerPorIdAsync(Person pPerson)
        {
            if (pPerson.PersonId <= 0)
                throw new Exception("El ID de persona no es válido.");

            return await PersonDAL.ObtenerPorIdAsync(pPerson);
        }

        /// <summary>
        /// Obtiene una lista de personas aplicando filtros opcionales.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Person"/> ordenados por apellido y nombre.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Person>> ObtenerTodosAsync(Person pPerson)
        {
            return await PersonDAL.ObtenerTodosAsync(pPerson);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de personas con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Person}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Person}"/> con la lista de personas encontradas
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Person>> BuscarAsync(PagedQuery<Person> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await PersonDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}